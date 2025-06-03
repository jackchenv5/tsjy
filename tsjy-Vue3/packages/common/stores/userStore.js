import { defineStore } from 'pinia'
import { onBeforeMount, ref } from 'vue'
import { useAppLocalStorage } from '../composables/appLocalStorage'

export const useUserStore = defineStore('user', () => {
  const user = ref({
    id: 0,
    userName: '',
    fullName: null,
    lastLogin: '',
  })

  const { localStorageGetItem, localStorageRemoveItem, localStorageSetItem } =
    useAppLocalStorage()

  const setUser = (value) => {
    user.value = value
    localStorageSetItem('user', JSON.stringify(user.value))
  }

  const clearUser = () => {
    localStorageRemoveItem('user')
  }

  onBeforeMount(() => {
    if (user.value.id <= 0) {
      user.value = JSON.parse(localStorageGetItem('user'))
    }
  })

  const updateFullName = (fullName) => {
    user.value.fullName = fullName
    localStorageSetItem('user', JSON.stringify(user.value))
  }

  return { user, setUser, clearUser, updateFullName }
})
