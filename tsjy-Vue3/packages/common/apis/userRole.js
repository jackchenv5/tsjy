import { http } from './http'

export const getUserRoleApi = (userId) => {
  return http.get(`/api/UserRole/${userId}`)
}

export const updateUserRoleApi = (userId, roleIds) => {
  return http.put(`/api/UserRole/${userId}`, roleIds)
}
