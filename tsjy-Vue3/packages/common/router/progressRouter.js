import { useNProgress } from '@vueuse/integrations/useNProgress'

export const useProgressRouter = (router) => {
  const { isLoading } = useNProgress()

  router.beforeEach(() => {
    isLoading.value = true
  })
  router.afterEach(() => {
    isLoading.value = false
  })
}
