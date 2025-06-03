import { useRouter } from 'vue-router'
import { useUserStore } from '../stores/userStore'
import { useAppLocalStorage } from './appLocalStorage'
import { ref } from 'vue'
import { ElMessage, useLocale } from 'element-plus'
import { loginApi } from '../apis/user'
import { useJwt } from './jwt'

export const useLogin = () => {
  const router = useRouter()

  const userStore = useUserStore()

  const { localStorageSetItem } = useAppLocalStorage()

  const loading = ref(false)

  const { t } = useLocale()

  /**
   *
   * @param {string} username 用户名
   * @param {string} password 应为密码哈希值
   * @param {boolean} rememberUsername 是否记住用户名，默认为 false
   */
  const login = async (username, password, rememberUsername = false) => {
    loading.value = true

    try {
      const res = await loginApi(username, password)
      if (res.data) {
        userStore.setUser(res.data)
      }

      localStorageSetItem('username', rememberUsername ? username : '')

      await router.push('/')
    } catch (e) {
      if (e.response.status === 401) {
        ElMessage({
          message: t('el.common.authentication.invalidUserNameOrPassword'),
          type: 'error',
        })
      }
    } finally {
      setTimeout(() => {
        loading.value = false
      }, 500)
    }
  }

  const { clearTokens } = useJwt()

  const logout = async () => {
    clearTokens()
    userStore.clearUser()
    await router.push('/login')
  }

  return { loading, login, logout }
}
