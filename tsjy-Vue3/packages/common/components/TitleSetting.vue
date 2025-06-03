<script setup>
import { useTitleStore } from '../stores/titleStore'
import { onMounted, ref } from 'vue'
import { getAppNameApi, updateAppNameApi } from '../apis/app'
import { ElMessage } from 'element-plus'
import { useLocale } from 'element-plus'

const titleStore = useTitleStore()
const appTitle = ref('')
const loading = ref(false)

const getAppName = async () => {
  if (loading.value) {
    return
  }

  loading.value = true

  try {
    const res = await getAppNameApi()
    const { value } = res.data
    appTitle.value = value
  } finally {
    loading.value = false
  }
}

onMounted(async () => {
  await getAppName()
})

const updateAppTitle = async () => {
  if (loading.value) {
    return
  }

  loading.value = true
  try {
    await updateAppNameApi(appTitle.value)
    titleStore.setTitle(appTitle.value)
    ElMessage({
      message: t('el.common.app.updateAppTitleSuccessful'),
      type: 'success',
    })
  } catch (e) {
    ElMessage({
      message: t('el.common.app.updateAppTitleFailed'),
      type: 'error',
    })
  } finally {
    loading.value = false
  }
}

const handleUpdateAppTitle = async () => {
  await updateAppTitle()
}

const { t } = useLocale()
</script>

<template>
  <ElForm
    label-position="top"
    v-loading="loading"
    @keydown.prevent.enter="handleUpdateAppTitle"
  >
    <ElFormItem :label="t('el.common.app.appTitle')">
      <ElInput v-model="appTitle" clearable></ElInput>
    </ElFormItem>
    <ElFormItem>
      <ElButton type="primary" :loading="loading" @click="handleUpdateAppTitle">
        {{ t('el.common.app.updateAppTitle') }}
      </ElButton>
    </ElFormItem>
  </ElForm>
</template>

<style scoped lang="scss"></style>
