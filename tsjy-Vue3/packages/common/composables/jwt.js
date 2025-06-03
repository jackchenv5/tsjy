import { useAppLocalStorage } from './appLocalStorage'

export const useJwt = () => {
  const { localStorageGetItem, localStorageRemoveItem, localStorageSetItem } =
    useAppLocalStorage()
  const decryptJwt = (token) => {
    token = token.replace(/_/g, '/').replace(/-/g, '+')
    let json = decodeURIComponent(encodeURI(window.atob(token.split('.')[1])))
    return JSON.parse(json)
  }

  const getToken = () => {
    return localStorageGetItem('token')
  }

  const getRefreshToken = () => {
    return localStorageGetItem('refreshToken')
  }

  const hasValidToken = () => {
    const tokenString = getToken()
    if (tokenString) {
      const token = decryptJwt(tokenString)
      const now = Date.now()
      return now / 1000 < token['exp']
    }
    return false
  }

  const hasValidRefreshToken = () => {
    const refreshTokenString = getRefreshToken()
    if (refreshTokenString) {
      const refreshToken = decryptJwt(refreshTokenString)
      const now = Date.now()
      return now / 1000 < refreshToken['exp']
    }
    return false
  }

  const isAuthenticated = () => {
    return hasValidToken() || hasValidRefreshToken()
  }

  const setTokens = (token, refreshToken) => {
    localStorageSetItem('token', token)
    localStorageSetItem('refreshToken', refreshToken)
  }

  const clearTokens = () => {
    localStorageRemoveItem('token')
    localStorageRemoveItem('refreshToken')
  }

  const getUserId = () => {
    if (!isAuthenticated()) {
      return ''
    }
    const token = decryptJwt(getToken())
    return token['id']
  }

  return {
    getToken,
    getRefreshToken,
    hasValidToken,
    hasValidRefreshToken,
    isAuthenticated,
    setTokens,
    clearTokens,
    getUserId,
  }
}
