import { http } from '#/common/apis/http'

export const getProductionBindingsApi = (facilityId) => {
  return http.get('/api/SawingProduction/Bindings', {
    params: {
      facilityId,
    },
  })
}
export const updateProductionBindingApi = (binding) => {
  return http.put('/api/SawingProduction/Bindings', binding)
}

export const getProductionHistoryApi = (dto) => {
  return http.post('/api/SawingProduction/GetProductionHistory', dto)
}
export const getProductionStatisticsApi = (dto) => {
  return http.post('/api/SawingProduction/GetProductionStatistics', dto)
}
