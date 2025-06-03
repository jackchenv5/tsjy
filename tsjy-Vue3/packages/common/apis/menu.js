import { http } from './http'

export const getMenuApi = () => {
  return http.get('/api/Menu')
}

export const addMenuApi = (menuInput) => {
  return http.post(`/api/Menu`, menuInput)
}

export const updateMenuApi = (id, menuInput) => {
  return http.put(`/api/Menu/${id}`, menuInput)
}

export const updateMenusApi = (menus) => {
  return http.put(`/api/Menu`, menus)
}

export const deleteMenuApi = (id) => {
  return http.delete(`/api/Menu/${id}`)
}
