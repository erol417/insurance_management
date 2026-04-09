# Lead DTO Catalog

## 1. Amac

Bu belge, `Lead Management` modulu icin kullanilacak DTO setini ayri ve net bir sekilde tanimlar.

## 2. Request DTO'lari

### `LeadCreateRequestDto`

- `potentialName`
- `phone`
- `email`
- `city`
- `district`
- `note`
- `leadSourceTypeId`
- `leadStatusTypeId`

### `LeadUpdateRequestDto`

- `potentialName`
- `phone`
- `email`
- `city`
- `district`
- `note`
- `leadStatusTypeId`
- `linkedAccountId`

### `LeadAssignmentRequestDto`

- `assignedEmployeeId`
- `priority`
- `dueDate`
- `assignmentNote`

## 3. Response DTO'lari

### `LeadListItemDto`

- `id`
- `code`
- `potentialName`
- `phone`
- `city`
- `leadStatus`
- `assignedEmployeeName`

### `LeadDetailDto`

- `id`
- `code`
- `potentialName`
- `phone`
- `email`
- `city`
- `district`
- `note`
- `leadStatus`
- `leadSource`
- `ownerUser`
- `linkedAccount`
- `currentAssignment`
- `convertedActivity`

## 4. Filter DTO

### `LeadFilterDto`

- `leadStatusTypeIds`
- `leadSourceTypeIds`
- `assignedEmployeeIds`
- `ownerUserIds`
- `city`
- `district`
- `onlyUnassigned`
- `onlyAssignedToMe`

## 5. Sonuc

Lead modulu icin DTO seti, call center olusturma, satis muduru atama ve saha personeli scoped gorunumu destekleyecek sekilde ayri dusunulmelidir.
