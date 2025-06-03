<script setup>
import { useDrawer } from '../composables/drawer'
import { ElMessage, useLocale } from 'element-plus'
import { ref, watch } from 'vue'
import { deleteRoleApi } from '../apis/role'

const props = defineProps(['modelValue', 'role'])
const emits = defineEmits(['update:modelValue', 'finish'])

const { showDrawer, drawerTitle, drawerSize, openDrawer, closeDrawer } =
  useDrawer()
const { t } = useLocale()

watch(
  () => props.modelValue,
  (val) => {
    if (val) {
      drawerTitle.value = t('el.common.role.deleteRole')
      openDrawer('400')
    }
  },
)

watch(showDrawer, (val) => {
  emits('update:modelValue', val)
})

const formData = ref({
  roleName: '',
})

watch(
  () => props.role,
  (val) => {
    if (val) {
      formData.value.roleName = val.roleName
    }
  },
)

const loading = ref(false)

const handleDeleteRole = async () => {
  if (loading.value) {
    return
  }

  loading.value = true
  try {
    await deleteRoleApi(props.role.id)
    ElMessage({
      message: t('el.common.role.deleteSuccessful'),
      type: 'success',
    })
    closeDrawer()
    formData.value = {
      roleName: '',
    }
    emits('finish')
  } catch (e) {
    ElMessage({
      message: t('el.common.role.deleteFailed'),
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
      @keydown.enter.prevent="handleDeleteRole"
      v-loading="loading || !role"
    >
      <ElFormItem :label="t('el.common.role.roleName')">
        <ElInput v-model="formData.roleName" clearable disabled></ElInput>
      </ElFormItem>
      <ElFormItem>
        <template #label>
          <p class="confirm-text">
            {{ t('el.common.role.deleteConfirm') }}
          </p>
        </template>
        <ElButton
          type="danger"
          :loading="loading"
          @click="handleDeleteRole"
          :disabled="formData.roleName.toLowerCase() === 'sysadmin'"
        >
          {{ t('el.common.role.delete') }}
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
