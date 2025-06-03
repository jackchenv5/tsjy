import { http } from '#/common/apis/http'

export const getFacilitiesApi = () => {
  return http.get('/api/Facility')
}
export const getFacilityApi = (id) => {
  return http.get(`/api/Facility/${id}`)
}
export const addFacilityApi = (facility) => {
  return http.post('/api/Facility', facility)
}
export const updateFacilityApi = (facility) => {
  return http.put('/api/Facility', facility)
}
export const deleteFacilityApi = (id) => {
  return http.delete('/api/Facility', {
    params: {
      id,
    },
  })
}

export const uploadFile = (formFiles) => {
  return http.post('/api/Facility/Upload', formFiles,{
    headers: {
      'Content-Type': 'multipart/form-data',
    },
  })
}
