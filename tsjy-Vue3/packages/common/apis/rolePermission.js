import { http } from './http'

export const getRolePermissionApi = (roleId) => {
  return http.get(`/api/RolePermission/${roleId}`)
}

export const updateRolePermissionApi = (roleId, permissionIds) => {
  return http.put(`/api/RolePermission/${roleId}`, permissionIds)
}
