import { http } from '#/common/apis/http'

export const getMaintainUsersApi = () => {
  return http.get('/api/MaintainUser/Users')
}
export const addMaintainUsersApi = (userIds) => {
  return http.post('/api/MaintainUser/Users', userIds)
}
export const removeMaintainUserApi = (id) => {
  return http.delete(`/api/MaintainUser/User`, {
    params: {
      id: id,
    },
  })
}
export const getMaintainRolesApi = () => {
  return http.get('/api/MaintainUser/Roles')
}
export const addMaintainRolesApi = (userIds) => {
  return http.post('/api/MaintainUser/Roles', userIds)
}
export const removeMaintainRoleApi = (id) => {
  return http.delete(`/api/MaintainUser/Role`, {
    params: {
      id: id,
    },
  })
}
