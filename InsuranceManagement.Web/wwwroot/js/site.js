document.addEventListener("DOMContentLoaded", () => {
  const contactStatus = document.querySelector("[data-contact-status]");
  const outcomeWrapper = document.querySelector("[data-outcome-wrapper]");

  function syncOutcomeVisibility() {
    if (!contactStatus || !outcomeWrapper) return;
    const visible = contactStatus.value === "Contacted";
    outcomeWrapper.style.display = visible ? "grid" : "none";
  }

  syncOutcomeVisibility();
  contactStatus?.addEventListener("change", syncOutcomeVisibility);
});
