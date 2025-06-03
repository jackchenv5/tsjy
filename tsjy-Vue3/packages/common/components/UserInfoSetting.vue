<script setup>
import { onMounted, ref } from 'vue'
import { ElMessage, useLocale } from 'element-plus'
import { useUserStore } from '../stores/userStore'
import { getOneUserApi, updateUserInfoApi } from '../apis/user'

const formData = ref({
  id: 0,
  username: '',
  fullName: '',
})

const { t } = useLocale()

const userStore = useUserStore()

const loading = ref(false)
const getOneUser = async () => {
  if (loading.value) {
    return
  }

  loading.value = true

  try {
    const res = await getOneUserApi(formData.value.id)
    formData.value = JSON.parse(JSON.stringify(res.data))
    userStore.setUser(res.data)
  } catch (e) {
    console.error(e)
  } finally {
    loading.value = false
  }
}

onMounted(async () => {
  formData.value.id = userStore.user.id
  await getOneUser()
})

const handleUpdateUserInfo = async () => {
  if (loading.value) {
    return
  }

  loading.value = true

  try {
    await updateUserInfoApi(formData.value.id, formData.value)
    userStore.updateFullName(formData.value.fullName)
    ElMessage({
      message: t('el.common.user.updateUserInfoSuccessful'),
      type: 'success',
    })
  } catch (e) {
    ElMessage({
      message: t('el.common.user.updateUserInfoFailed'),
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
    <ElFormItem :label="t('el.common.user.username')">
      <ElInput disabled v-model="formData.username"></ElInput>
    </ElFormItem>
    <ElFormItem :label="t('el.common.user.fullName')">
      <ElInput v-model="formData.fullName"></ElInput>
    </ElFormItem>
    <ElFormItem>
      <ElButton type="primary" :loading="loading" @click="handleUpdateUserInfo">
        {{ t('el.common.user.updateUserInfo') }}
      </ElButton>
    </ElFormItem>
  </ElForm>
</template>

<style scoped lang="scss"></style>
