import { ref, toValue } from 'vue'

export const useValidateRoleName = () => {
  const validRoleName = ref(false)

  const validateRoleName = (username) => {
    const pattern = /^[a-zA-Z][a-zA-Z0-9_]{3,31}$/
    if (toValue(username)) {
      validRoleName.value = pattern.test(toValue(username))
    } else {
      validRoleName.value = false
    }
  }

  return { validRoleName, validateRoleName }
}
