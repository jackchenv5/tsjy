import { useNotFoundRouter } from './notfoundRouter'
import { useAuthenticationRouter } from './authenticationRouter'
import { useProgressRouter } from './progressRouter'
import { useCustomHomeRouter } from './customHomePageRouter'

export const useCommonRouter = (router) => {
  useNotFoundRouter(router)
  useAuthenticationRouter(router)
  useProgressRouter(router)
  useCustomHomeRouter(router)
}
