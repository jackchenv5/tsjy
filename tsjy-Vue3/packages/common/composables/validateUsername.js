import { ref, toValue } from 'vue'

export const useValidateUsername = () => {
  const validUsername = ref(false)

  const validateUsername = (username) => {
    const pattern = /^[a-zA-Z][a-zA-Z0-9_]{3,31}$/
    if (toValue(username)) {
      validUsername.value = pattern.test(toValue(username))
    } else {
      validUsername.value = false
    }
  }

  return { validUsername, validateUsername }
}
