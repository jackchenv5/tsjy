import { http } from '#/common/apis/http'

export const getPartApi = (facilityId) => {
  return http.get('/api/SawingPart', {
    params: {
      facilityId,
    },
  })
}
export const addPartApi = (parts) => {
  return http.post('/api/SawingPart', parts)
}
export const updatePartApi = (part) => {
  return http.put('/api/SawingPart', part)
}

export const updatePartHistoriesApi = (part) => {
  return http.put('/api/SawingPart/PutHistories', part)
}

export const deletePartApi = (id) => {
  return http.delete('/api/SawingPart', {
    params: {
      id,
    },
  })
}

export const deletePartHistoriesApi = (id) => {
  return http.delete('/api/SawingPart/DeleteHistory', {
    params: {
      id,
    },
  })
}

export const getPartMaintainHistoryApi = (dto) => {
  return http.post('/api/SawingPart/GetMaintainHistory', dto)
}
export const addPartMaintainHistoryApi = (dto) => {
  return http.post('/api/SawingPart/AddMaintainHistory', dto)
}
