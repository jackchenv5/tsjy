export const useNotFoundRouter = (router) => {
  // 处理未找到匹配的路由的情况
  router.beforeEach((to, from, next) => {
    if (to.matched.length === 0) {
      router.replace('/404')
    }
    next()
  })
}
