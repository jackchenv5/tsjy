import { http } from '#/common/apis/http'


export const addMotorApi = (motor) => {
  return http.post('/api/SawingMotor', motor)
}
export const getMotorsApi = (facilityId) => {
  return http.get('/api/SawingMotor', {
    params: {
      facilityId,
    },
  })
}
export const updateMotorApi = (motor) => {
  return http.put('/api/SawingMotor', motor)
}
export const deleteMotorApi = (motorId) => {
  return http.delete('/api/SawingMotor', {
    params: {
      motorId,
    },
  })
}

export const getMotorBindingApi = (motorId) => {
  return http.get(`/api/SawingMotor/Binding/${motorId}`)
}
export const updateMotorBindingApi = (binding) => {
  return http.put('/api/SawingMotor/Binding', binding)
}

export const getMotorDataApi = (motorId) => {
  return http.get('/api/SawingMotor/Data', {
    params: {
      motorId,
    },
  })
}
export const getMotorHistoryDataApi = (motorId, startTime, endTime) => {
  return http.get('/api/SawingMotor/Data/History', {
    params: {
      motorId,
      startTime,
      endTime,
    },
  })
}
export const getRollDiameterDataApi = (motorId) => {
  return http.get('/api/SawingMotor/RollDiameter', {
    params: {
      motorId,
    },
  })
}
export const getRollDiameterHistoryDataApi = (motorId, startTime, endTime) => {
  return http.get(`/api/SawingMotor/RollDiameter/History`, {
    params: { motorId, startTime, endTime }
  })
}