import { http } from './http'

export const getRoleMenuApi = (roleId) => {
  return http.get(`/api/RoleMenu/${roleId}`)
}

export const getMenuApi = () => {
  return http.get('/api/RoleMenu')
}

export const updateRoleMenuApi = (roleId, menuIds) => {
  return http.put(`/api/RoleMenu/${roleId}`, menuIds)
}
