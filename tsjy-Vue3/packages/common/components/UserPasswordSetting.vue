<script setup>
import { ref } from 'vue'
import { ElMessage, useLocale } from 'element-plus'
import { useValidatePassword } from '../composables/validatePassword'
import { sha256 } from 'js-sha256'
import { updatePasswordApi } from '../apis/user'
import { useUserStore } from '../stores/userStore'

const formData = ref({
  password: '',
  newPassword: '',
  confirmNewPassword: '',
})

const { t } = useLocale()

const loading = ref(false)
const { validPassword, validatePassword } = useValidatePassword()
const userStore = useUserStore()

const handleUpdatePassword = async () => {
  if (loading.value) {
    return
  }

  validatePassword(formData.value.newPassword)
  if (!validPassword.value) {
    ElMessage({
      message: t('el.common.user.invalidPassword'),
      type: 'error',
    })
    return
  }

  if (formData.value.newPassword !== formData.value.confirmNewPassword) {
    ElMessage({
      message: t('el.common.user.passwordNotMatch'),
      type: 'error',
    })
    return
  }

  const passwordHash = sha256(formData.value.password)
  const newPasswordHash = sha256(formData.value.newPassword)
  const confirmNewPasswordHash = sha256(formData.value.confirmNewPassword)

  const passwordInput = {
    password: passwordHash,
    newPassword: newPasswordHash,
    confirmNewPassword: confirmNewPasswordHash,
  }

  loading.value = true

  try {
    await updatePasswordApi(userStore.user.id, passwordInput)
    ElMessage({
      message: t('el.common.user.updatePasswordSuccessful'),
      type: 'success',
    })
    formData.value = {
      password: '',
      newPassword: '',
      confirmNewPassword: '',
    }
  } catch (e) {
    ElMessage({
      message: t('el.common.user.updatePasswordFailed'),
      type: 'error',
    })
    console.error(e)
  } finally {
    loading.value = false
  }
}
</script>

<template>
  <ElForm label-position="top" v-model="formData" v-loading="loading">
    <ElFormItem :label="t('el.common.user.currentPassword')">
      <ElInput show-password v-model="formData.password"></ElInput>
    </ElFormItem>
    <ElFormItem :label="t('el.common.user.newPassword')">
      <ElInput show-password v-model="formData.newPassword"></ElInput>
    </ElFormItem>
    <ElFormItem :label="t('el.common.user.confirmNewPassword')">
      <ElInput show-password v-model="formData.confirmNewPassword"></ElInput>
    </ElFormItem>
    <ElFormItem>
      <ElButton type="primary" :loading="loading" @click="handleUpdatePassword">
        {{ t('el.common.user.updatePassword') }}
      </ElButton>
    </ElFormItem>
  </ElForm>
</template>

<style scoped lang="scss"></style>
