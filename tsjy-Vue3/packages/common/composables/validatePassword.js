import { ref, toValue } from 'vue'

export const useValidatePassword = () => {
  const validPassword = ref(false)

  const validatePassword = (password) => {
    const pattern = /^[a-zA-Z0-9!@#$%^&*()_+.,?;:|<>~=-]{6,32}$/

    if (toValue(password)) {
      validPassword.value = pattern.test(toValue(password))
    } else {
      validPassword.value = false
    }
  }

  return { validPassword, validatePassword }
}
