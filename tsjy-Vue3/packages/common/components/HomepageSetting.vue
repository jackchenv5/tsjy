<script setup>
import { onMounted, ref } from 'vue'
import { getCustomHomePageApi, updateCustomHomePageApi } from '../apis/app'
import SelectRoute from './SelectRoute.vue'
import { ElMessage, useLocale } from 'element-plus'

const { t } = useLocale()

const customHomePage = ref('')
const loading = ref(false)

const getCustomHomePage = async () => {
  if (loading.value) {
    return
  }

  loading.value = true

  try {
    const res = await getCustomHomePageApi()
    const { value } = res.data
    customHomePage.value = value
  } finally {
    loading.value = false
  }
}

onMounted(async () => {
  await getCustomHomePage()
})

const updateCustomHomePage = async () => {
  if (loading.value) {
    return
  }

  loading.value = true
  try {
    await updateCustomHomePageApi(customHomePage.value)
    ElMessage({
      message: t('el.common.app.updateDefaultPageSuccessful'),
      type: 'success',
    })
  } catch (e) {
    ElMessage({
      message: t('el.common.app.updateDefaultPageFailed'),
      type: 'error',
    })
  } finally {
    loading.value = false
  }
}

const handleUpdateCustomHomePage = async () => {
  await updateCustomHomePage()
}
</script>

<template>
  <ElForm
    label-position="top"
    v-loading="loading"
    @keydown.prevent.enter="handleUpdateCustomHomePage"
  >
    <ElFormItem :label="t('el.common.app.defaultPage')">
      <SelectRoute v-model="customHomePage"></SelectRoute>
    </ElFormItem>
    <ElFormItem>
      <ElButton
        type="primary"
        :loading="loading"
        @click="handleUpdateCustomHomePage"
      >
        {{ t('el.common.app.updateDefaultPage') }}
      </ElButton>
    </ElFormItem>
  </ElForm>
</template>

<style scoped lang="scss"></style>
