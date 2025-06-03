import { http } from '#/common/apis/http'

export const addCraftGroupApi = (facilityId) => {
  return http.get('/api/SawingCraft/AddGroup', {
    params: {
      facilityId,
    },
  })
}
export const deleteCraftGroupApi = (facilityId) => {
  return http.delete('/api/SawingCraft/DeleteGroup', {
    params: {
      facilityId,
    },
  })
}
export const getCraftBindingsApi = (facilityId) => {
  return http.get('/api/SawingCraft/Binding', {
    params: {
      facilityId,
    },
  })
}
export const updateCraftBindingApi = (updateBindingDto) => {
  return http.put('/api/SawingCraft/Binding', updateBindingDto)
}
export const getCraftHistoryApi = (dto) => {
  return http.post('/api/SawingCraft/HistoryData', dto)
}
