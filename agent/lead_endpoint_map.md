# Lead Endpoint Map

## 1. Amac

Bu belge, `Lead Management` modulu icin endpoint ve DTO eslesmelerini toplar.

## 2. Endpoint Listesi

| Method | Endpoint | Request DTO | Response DTO | Roles | MVP | Notes |
|---|---|---|---|---|---|---|
| GET | `/api/leads` | `LeadFilterDto` + query + pagination | `PagedResultDto<LeadListItemDto>` | Admin, Manager, SalesManager, CallCenter, Operations | Yes | Lead havuzu |
| GET | `/api/leads/{id}` | None | `LeadDetailDto` | Admin, Manager, SalesManager, CallCenter, Operations, FieldSales scoped | Yes | Detay |
| POST | `/api/leads` | `LeadCreateRequestDto` | `LeadDetailDto` | Admin, CallCenter, Operations | Yes | Yeni lead |
| PATCH | `/api/leads/{id}` | `LeadUpdateRequestDto` | `LeadDetailDto` | Admin, CallCenter, Operations | Yes | Bilgi ve status guncelleme |
| POST | `/api/leads/{id}/assignments` | `LeadAssignmentRequestDto` | `LeadDetailDto` | Admin, SalesManager, Manager | Yes | Saha atamasi |
| GET | `/api/leads/my-assignments` | query optional | `PagedResultDto<LeadListItemDto>` | FieldSales | Yes | Atanmis leadlerim |

## 3. Lookup Eki

- `GET /api/lookups/lead-status-types`
- `GET /api/lookups/lead-source-types`

## 4. Sonuc

Lead modulu, call center -> satis muduru -> saha personeli hattini API seviyesinde ayri ve net sekilde temsil etmelidir.
