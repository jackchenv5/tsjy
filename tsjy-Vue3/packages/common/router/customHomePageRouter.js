import { getCustomHomePageApi } from '../apis/app'

export const useCustomHomeRouter = (router) => {
  router.beforeEach(async (to, from, next) => {
    if (to.path === '/') {
      try {
        const res = await getCustomHomePageApi()
        const { value } = res.data
        if (value === '/') {
          next('/404')
        } else {
          next(value)
        }
      } catch (e) {
        next('/404')
      }
    } else {
      next()
    }
  })
}
