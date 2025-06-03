import { useJwt } from './jwt'
import { useAppLocalStorage } from './appLocalStorage'
import { useAuthenticationStore } from '../stores/authenticationStore'
import { ElMessage } from 'element-plus'
import { authenticationLang } from '../langs/authentication.js'
import { useRouter } from 'vue-router'

export const useAuthenticationTip = () => {
  const { getToken, getRefreshToken } = useJwt()

  const { localStorageGetItem } = useAppLocalStorage()

  const token = getToken()
  const refreshToken = getRefreshToken()
  const lang = localStorageGetItem('lang')

  let msg
  if (token || refreshToken) {
    msg = t(lang, true)
  } else {
    msg = t(lang, false)
  }

  const authenticationStore = useAuthenticationStore()

  if (authenticationStore.showMsg) {
    authenticationStore.setShowMsg(false)
    ElMessage({
      message: msg,
      type: 'error',
    })
  }

  const router = useRouter()
  setTimeout(async () => {
    if (router) {
      await router.push('/login')
    } else {
      window.location.href = '/#/login'
    }
  }, 2000)
}

const t = (lang = 'en', needLoginAgain = false) => {
  switch (lang) {
    case 'en':
      return needLoginAgain
        ? authenticationLang.en.el.authentication.needLoginAgain
        : authenticationLang.en.el.authentication.needLogin
    case 'zh-cn':
    default:
      return needLoginAgain
        ? authenticationLang.zhCn.el.authentication.needLoginAgain
        : authenticationLang.zhCn.el.authentication.needLogin
  }
}
