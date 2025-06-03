<script setup>
import { ref, watch } from 'vue'
import { useDrawer } from '../composables/drawer'
import { ElMessage, useLocale } from 'element-plus'
import { useValidateUsername } from '../composables/validateUsername'
import { useValidatePassword } from '../composables/validatePassword'
import { sha256 } from 'js-sha256'
import { addUserApi } from '../apis/user'

const props = defineProps(['modelValue'])
const emits = defineEmits(['update:modelValue', 'finish'])

const { showDrawer, drawerTitle, drawerSize, openDrawer, closeDrawer } =
  useDrawer()
const { t } = useLocale()

watch(
  () => props.modelValue,
  (val) => {
    if (val) {
      drawerTitle.value = t('el.common.user.addUser')
      openDrawer('400')
    }
  },
)

watch(showDrawer, (val) => {
  emits('update:modelValue', val)
})

const formData = ref({
  username: '',
  fullName: '',
  password: '',
})
const loading = ref(false)

const { validUsername, validateUsername } = useValidateUsername()
const { validPassword, validatePassword } = useValidatePassword()

const handleAddUser = async () => {
  if (loading.value) {
    return
  }

  validateUsername(formData.value.username)
  if (!validUsername.value) {
    ElMessage({
      message: t('el.common.user.invalidUsername'),
      type: 'error',
    })
  }

  validatePassword(formData.value.password)
  if (!validPassword.value) {
    ElMessage({
      message: t('el.common.user.invalidPassword'),
      type: 'error',
    })
  }

  if (!validUsername.value || !validPassword.value) {
    return
  }

  const userInput = {
    username: formData.value.username,
    fullName: formData.value.fullName,
    password: sha256(formData.value.password),
  }

  loading.value = true

  try {
    await addUserApi(userInput)
    ElMessage({
      message: t('el.common.user.addSuccessful'),
      type: 'success',
    })
    closeDrawer()
    formData.value = {
      username: '',
      fullName: '',
      password: '',
    }
    emits('finish')
  } catch (e) {
    ElMessage({
      message: t('el.common.user.addFailed'),
      type: 'error',
    })
    console.error(e)
  } finally {
    loading.value = false
  }
}
</script>

<template>
  <ElDrawer :title="drawerTitle" v-model="showDrawer" :size="drawerSize">
    <ElForm
      v-model="formData"
      label-position="top"
      @keydown.enter.prevent="handleAddUser"
      v-loading="loading"
    >
      <ElFormItem :label="t('el.common.user.username')">
        <ElInput v-model="formData.username" clearable></ElInput>
        <ul class="invalid-msg">
          <li>{{ t('el.common.user.usernameRule1') }}</li>
          <li>{{ t('el.common.user.usernameRule2') }}</li>
          <li>{{ t('el.common.user.usernameRule3') }}</li>
        </ul>
      </ElFormItem>
      <ElFormItem :label="t('el.common.user.fullName')">
        <ElInput v-model="formData.fullName" clearable></ElInput>
      </ElFormItem>
      <ElFormItem :label="t('el.common.user.password')">
        <ElInput v-model="formData.password" show-password clearable></ElInput>
        <ul class="invalid-msg">
          <li>{{ t('el.common.user.passwordRule') }}</li>
        </ul>
      </ElFormItem>
      <ElFormItem>
        <ElButton type="primary" :loading="loading" @click="handleAddUser">
          {{ t('el.common.user.add') }}
        </ElButton>
      </ElFormItem>
    </ElForm>
  </ElDrawer>
</template>

<style scoped lang="scss">
.invalid-msg {
  font-size: 1rem;
  line-height: 2rem;
  color: var(--el-color-danger);
  padding-left: 1.6rem;
  margin-bottom: 0;
}
</style>
