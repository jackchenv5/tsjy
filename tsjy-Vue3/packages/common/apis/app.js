import { http } from './http.js'

export const getAppNameApi = () => {
  return http.get('/api/Setting/AppName')
}

export const updateAppNameApi = (name) => {
  return http.put('/api/Setting/AppName', name, {
    headers: {
      'Content-Type': 'application/json',
    },
  })
}

export const getCustomHomePageApi = () => {
  return http.get('/api/Setting/HomePage')
}

export const updateCustomHomePageApi = (homePage) => {
  return http.put('/api/Setting/HomePage', homePage, {
    headers: {
      'Content-Type': 'application/json',
    },
  })
}
