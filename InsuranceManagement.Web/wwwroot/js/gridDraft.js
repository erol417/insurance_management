(function (window) {
    function normalizeValue(value) {
        if (value === undefined || value === null) {
            return "";
        }

        return String(value).trim();
    }

    function isEmptyRecord(record) {
        if (!record) {
            return true;
        }

        return Object.keys(record).every(key => key === "id" || normalizeValue(record[key]) === "");
    }

    function recordsEqual(left, right) {
        const keys = new Set([...(Object.keys(left || {})), ...(Object.keys(right || {}))]);
        for (const key of keys) {
            if (key === "id") {
                continue;
            }

            if (normalizeValue(left?.[key]) !== normalizeValue(right?.[key])) {
                return false;
            }
        }

        return true;
    }

    function getStore(key) {
        try {
            return JSON.parse(localStorage.getItem(key) || '{"updated":{},"created":{}}');
        } catch {
            return { updated: {}, created: {} };
        }
    }

    function setStore(key, store) {
        localStorage.setItem(key, JSON.stringify(store));
    }

    function hasDraft(store) {
        return Object.keys(store.updated || {}).length > 0 || Object.keys(store.created || {}).length > 0;
    }

    function defaultCollectRowData(row) {
        const data = {};
        row.querySelectorAll("[data-field]").forEach(input => {
            data[input.dataset.field] = input.value;
        });
        return data;
    }

    function defaultApplyDraft(row, draft) {
        row.querySelectorAll("[data-field]").forEach(input => {
            const field = input.dataset.field;
            if (draft[field] !== undefined) {
                input.value = draft[field];
            }
        });
    }

    function toggleRow(rowId) {
        const row = document.getElementById(rowId);
        if (!row) return;
        row.style.display = row.style.display === "none" ? "table-row" : "none";
    }

    function create(options) {
        let suppressLeaveWarning = false;
        const collectRowData = options.collectRowData || defaultCollectRowData;
        const applyDraft = options.applyDraft || defaultApplyDraft;
        const onRowReady = options.onRowReady || function () { };
        const onInsertRowReady = options.onInsertRowReady || function () { };
        const afterDraftApplied = options.afterDraftApplied || function () { };
        const rowSelector = options.rowSelector;
        const rowIdAttribute = options.rowIdAttribute;
        const insertRowId = options.insertRowId;
        const baselineById = {};
        let insertRowBaseline = {};

        function readStore() {
            return getStore(options.storageKey);
        }

        function writeStore(store) {
            setStore(options.storageKey, store);
        }

        function pruneStore(store) {
            const nextStore = {
                updated: { ...(store.updated || {}) },
                created: { ...(store.created || {}) }
            };

            Object.keys(nextStore.updated).forEach(key => {
                const baseline = baselineById[key];
                if (baseline && recordsEqual(nextStore.updated[key], baseline)) {
                    delete nextStore.updated[key];
                }
            });

            Object.keys(nextStore.created).forEach(key => {
                if (isEmptyRecord(nextStore.created[key]) || recordsEqual(nextStore.created[key], insertRowBaseline)) {
                    delete nextStore.created[key];
                }
            });

            return nextStore;
        }

        function syncStore() {
            const pruned = pruneStore(readStore());
            writeStore(pruned);
            return pruned;
        }

        function hasAnyDraft() {
            return hasDraft(syncStore());
        }

        function bindExistingRows() {
            document.querySelectorAll(rowSelector).forEach(row => {
                onRowReady(row);
                const id = row.getAttribute(rowIdAttribute);
                baselineById[id] = collectRowData(row);
                row.querySelectorAll("[data-field]").forEach(input => {
                    const handler = function () {
                        onRowReady(row);
                        const store = readStore();
                        store.updated[id] = { id: Number(id), ...collectRowData(row) };
                        writeStore(pruneStore(store));
                    };
                    input.addEventListener("input", handler);
                    input.addEventListener("change", handler);
                });
            });
        }

        function saveNewDraft() {
            const row = document.getElementById(insertRowId);
            if (!row) return;
            onInsertRowReady(row);
            const values = collectRowData(row);
            const hasValue = Object.values(values).some(x => normalizeValue(x) !== "");
            const store = readStore();
            if (!hasValue) {
                delete store.created["new-top"];
            } else {
                store.created["new-top"] = values;
            }
            writeStore(pruneStore(store));
        }

        function bindInsertRow() {
            const row = document.getElementById(insertRowId);
            if (!row) return;
            onInsertRowReady(row);
            insertRowBaseline = collectRowData(row);
            row.querySelectorAll("[data-field]").forEach(input => {
                input.addEventListener("input", saveNewDraft);
                input.addEventListener("change", saveNewDraft);
            });
        }

        function applyDraftToExistingRows() {
            const store = readStore();
            document.querySelectorAll(rowSelector).forEach(row => {
                const draft = store.updated[row.getAttribute(rowIdAttribute)];
                if (!draft) return;
                applyDraft(row, draft);
                afterDraftApplied(row, draft);
            });
        }

        function applyDraftToInsertRow() {
            const row = document.getElementById(insertRowId);
            const store = readStore();
            const draft = store.created["new-top"];
            if (!row || !draft) return;
            row.style.display = "table-row";
            applyDraft(row, draft);
            afterDraftApplied(row, draft);
        }

        function serializeDraftForSave() {
            const store = syncStore();
            const payload = [];
            Object.keys(store.updated || {}).forEach(key => payload.push(store.updated[key]));
            Object.keys(store.created || {}).forEach(key => payload.push(store.created[key]));
            return payload;
        }

        function submitDraft() {
            const payload = serializeDraftForSave();
            if (!payload.length) {
                alert("Kaydedilecek degisiklik bulunamadi.");
                return;
            }

            suppressLeaveWarning = true;
            document.getElementById(options.payloadInputId).value = JSON.stringify(payload);
            document.getElementById(options.formId).submit();
        }

        function clearDraft() {
            if (!hasAnyDraft()) return;
            if (!confirm("Kaydedilmemis tum degisiklikler silinsin mi?")) return;
            localStorage.removeItem(options.storageKey);
            suppressLeaveWarning = true;
            location.reload();
        }

        window.addEventListener("beforeunload", function (event) {
            if (!suppressLeaveWarning && hasAnyDraft()) {
                event.preventDefault();
                event.returnValue = "";
            }
        });

        document.addEventListener("DOMContentLoaded", function () {
            if (options.clearOnLoad) {
                localStorage.removeItem(options.storageKey);
            }

            bindExistingRows();
            bindInsertRow();
            syncStore();
            applyDraftToExistingRows();
            applyDraftToInsertRow();
            syncStore();

            const saveButton = document.getElementById(options.saveButtonId);
            if (saveButton) {
                saveButton.addEventListener("click", submitDraft);
            }

            const clearButton = document.getElementById(options.clearButtonId);
            if (clearButton) {
                clearButton.addEventListener("click", clearDraft);
            }

            document.querySelectorAll(options.pageLinkSelector || ".grid-page-link").forEach(link => {
                link.addEventListener("click", function () {
                    suppressLeaveWarning = true;
                });
            });
        });

        return {
            clearDraft,
            submitDraft,
            hasAnyDraft
        };
    }

    window.InsuranceGridDraft = {
        create,
        toggleRow
    };
})(window);
