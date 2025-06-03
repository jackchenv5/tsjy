import axios from 'axios'
import { useJwt } from '../composables/jwt.js'
import { useAuthenticationTip } from '../composables/authenticationTip.js'

axios.defaults.baseURL = import.meta.env.BASE_URL

axios.interceptors.response.use(
  (res) => {
    const { setTokens } = useJwt()
    const token = res.headers['access-token']
    const refreshToken = res.headers['refresh-token']

    if (token && refreshToken) {
      setTokens(token, refreshToken)
    }

    return res
  },
  (error) => {
    if (!error) {
      return Promise.reject(error)
    }
    if (error.response.status === 401) {
      const url = error.response.config.url
      if (url === '/api/User/Login') {
        // 调用的认证接口，不额外处理
      } else {
        // 未认证，重新跳转到登录页面
        useAuthenticationTip()
      }
    }
    return Promise.reject(error)
  },
)

axios.interceptors.request.use(
  (req) => {
    const { hasValidToken, hasValidRefreshToken, getToken, getRefreshToken } =
      useJwt()

    if (req && req.headers) {
      if (hasValidToken()) {
        req.headers['Authorization'] = `Bearer ${getToken()}`
        return req
      }

      if (hasValidRefreshToken()) {
        req.headers['Authorization'] = `Bearer ${getToken()}`
        req.headers['X-Refresh-Token'] = `Bearer ${getRefreshToken()}`
        return req
      }
    }

    return req
  },
  (error) => {
    return Promise.reject(error)
  },
)

const http = axios

export { http }
