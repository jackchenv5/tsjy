import { http } from '#/common/apis/http'

export const addAlarmDefinitionApi = (definition) => {
  return http.post(`/api/SawingMachineAlarm/Definition`, definition)
}
export const addAlarmDefinitionsApi = (definitions) => {
  return http.post(`/api/SawingMachineAlarm/Definitions`, definitions)
}
export const getAlarmDefinitionsApi = (pageSelector) => {
  return http.get(`/api/SawingMachineAlarm/Definitions`,{
    params: { pageIndex: pageSelector.pageIndex, pageSize: pageSelector.pageSize },
  })
}
export const updateAlarmDefinitionApi = (definition) => {
  return http.put(`/api/SawingMachineAlarm/Definition`, definition)
}
export const deleteAlarmDefinitionApi = (definitionId) => {
  return http.delete(`/api/SawingMachineAlarm/Definition/${definitionId}`)
}

export const getCurrentAlarmsApi = () => {
  return http.get('/api/SawingMachineAlarm/CurrentAlarms')
}
export const getAlarmHistoryApi = (param) => {
  return http.post('/api/SawingMachineAlarm/GetHistory', param)
}
export const getAlarmCountAsync = (param) => {
  return http.post('/api/SawingMachineAlarm/GetAlarmCount', param)
}
