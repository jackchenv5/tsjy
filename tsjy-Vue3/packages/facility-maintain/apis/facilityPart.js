import { http } from '#/common/apis/http'

export const importFacilityPartApi = (parts) => {
  return http.post('/api/FacilityPart/Import', parts)
}
export const getFacilityPartsApi = (facilityId, name, type) => {
  return http.get('/api/FacilityPart', {
    params: {
      facilityId,
      name,
      type,
    },
  })
}
