<script setup>
import { useDrawer } from '../composables/drawer'
import { ElMessage, useLocale } from 'element-plus'
import { ref, watch } from 'vue'
import { deleteUserApi } from '../apis/user'

const props = defineProps(['modelValue', 'user'])
const emits = defineEmits(['update:modelValue', 'finish'])
const { showDrawer, drawerTitle, drawerSize, openDrawer, closeDrawer } =
  useDrawer()
const { t } = useLocale()

watch(
  () => props.modelValue,
  (val) => {
    if (val) {
      drawerTitle.value = t('el.common.user.deleteUser')
      openDrawer('400')
    }
  },
)

watch(showDrawer, (val) => {
  emits('update:modelValue', val)
})

watch(
  () => props.user,
  (val) => {
    if (val) {
      formData.value.id = val.id
      formData.value.username = val.username
      formData.value.fullName = val.fullName
    }
  },
)

const formData = ref({
  id: 0,
  username: '',
  fullName: '',
})
const loading = ref(false)

const handleDeleteUser = async () => {
  if (loading.value) {
    return
  }

  loading.value = true

  try {
    await deleteUserApi(formData.value.id)
    ElMessage({
      message: t('el.common.user.deleteSuccessful'),
      type: 'success',
    })
    closeDrawer()
    formData.value = {
      id: 0,
      username: '',
      fullName: '',
    }
    emits('finish')
  } catch (e) {
    ElMessage({
      message: t('el.common.user.deleteFailed'),
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
      @keydown.enter.prevent="handleDeleteUser"
      v-loading="loading || !user"
    >
      <ElFormItem :label="t('el.common.user.username')">
        <ElInput v-model="formData.username" disabled></ElInput>
      </ElFormItem>
      <ElFormItem :label="t('el.common.user.fullName')">
        <ElInput v-model="formData.fullName" disabled></ElInput>
      </ElFormItem>
      <ElFormItem>
        <template #label>
          <p class="confirm-text">
            {{ t('el.common.user.deleteConfirm') }}
          </p>
        </template>
        <ElButton
          type="danger"
          :loading="loading"
          @click="handleDeleteUser"
          :disabled="formData.username.toLowerCase() === 'sysadmin'"
        >
          {{ t('el.common.user.delete') }}
        </ElButton>
      </ElFormItem>
    </ElForm>
  </ElDrawer>
</template>

<style scoped lang="scss">
.confirm-text {
  color: var(--el-color-danger);
}
</style>
