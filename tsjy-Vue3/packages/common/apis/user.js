import { http } from './http.js'

/**
 * 登录接口
 * @param {string} username 用户名
 * @param {string} password 应为密码哈希值
 * @returns {Promise}
 */
export const loginApi = (username, password) => {
  return http.post('/api/User/Login', { username, password })
}

export const getUserApi = (pageIndex, pageSize) => {
  return http.get('/api/User', {
    params: { pageIndex: pageIndex, pageSize: pageSize },
  })
}

export const addUserApi = (userInput) => {
  return http.post('/api/User', userInput)
}

export const updateUserApi = (id, userInput) => {
  return http.put(`/api/User/${id}`, userInput)
}

export const deleteUserApi = (id) => {
  return http.delete(`/api/User/${id}`)
}

export const getOneUserApi = (id) => {
  return http.get(`/api/User/${id}`)
}

export const updateUserInfoApi = (id, userInfoInput) => {
  return http.put(`/api/User/Info/${id}`, userInfoInput)
}

export const updatePasswordApi = (id, passwordInput) => {
  return http.put(`/api/User/Password/${id}`, passwordInput)
}
