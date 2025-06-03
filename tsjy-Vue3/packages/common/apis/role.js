import { http } from './http'

export const getRoleApi = (pageIndex, pageSize) => {
  return http.get('/api/Role', {
    params: { pageIndex: pageIndex, pageSize: pageSize },
  })
}

export const addRoleApi = (roleInput) => {
  return http.post('/api/Role', roleInput)
}

export const updateRoleApi = (id, roleInput) => {
  return http.put(`/api/Role/${id}`, roleInput)
}

export const deleteRoleApi = (id) => {
  return http.delete(`/api/Role/${id}`)
}
