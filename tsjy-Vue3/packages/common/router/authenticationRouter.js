import { useJwt } from '../composables/jwt.js'
import { useAuthenticationTip } from '../composables/authenticationTip.js'

export const useAuthenticationRouter = (router) => {
  const { isAuthenticated } = useJwt()

  router.beforeEach((to, from, next) => {
    if (to.matched.some((route) => route.meta['anonymous'] === undefined)) {
      // 未指定允许匿名访问
      if (isAuthenticated()) {
        next()
      } else {
        useAuthenticationTip()
      }
    } else {
      next()
    }
  })
}
