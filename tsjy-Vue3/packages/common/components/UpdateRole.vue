<script setup>
import { useDrawer } from '../composables/drawer'
import { ElMessage, useLocale } from 'element-plus'
import { ref, watch } from 'vue'
import { useValidateRoleName } from '../composables/validateRoleName'
import { updateRoleApi } from '../apis/role'

const props = defineProps(['modelValue', 'role'])
const emits = defineEmits(['update:modelValue', 'finish'])

const { showDrawer, drawerTitle, drawerSize, openDrawer, closeDrawer } =
  useDrawer()
const { t } = useLocale()

watch(
  () => props.modelValue,
  (val) => {
    if (val) {
      drawerTitle.value = t('el.common.role.updateRole')
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

const { validRoleName, validateRoleName } = useValidateRoleName()

const handleUpdateRole = async () => {
  if (loading.value) {
    return
  }

  validateRoleName(formData.value.roleName)
  if (!validRoleName.value) {
    ElMessage({
      message: t('el.common.role.invalidRoleName'),
      type: 'error',
    })
    return
  }

  loading.value = true
  try {
    await updateRoleApi(props.role.id, formData.value)
    ElMessage({
      message: t('el.common.role.updateSuccessful'),
      type: 'success',
    })
    closeDrawer()
    formData.value = {
      roleName: '',
    }
    emits('finish')
  } catch (e) {
    ElMessage({
      message: t('el.common.role.updateFailed'),
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
      @keydown.enter.prevent="handleUpdateRole"
    >
      <ElFormItem :label="t('el.common.role.roleName')">
        <ElInput v-model="formData.roleName" clearable></ElInput>
        <ul class="invalid-msg">
          <li>{{ t('el.common.role.roleNameRule1') }}</li>
          <li>{{ t('el.common.role.roleNameRule2') }}</li>
          <li>{{ t('el.common.role.roleNameRule3') }}</li>
        </ul>
      </ElFormItem>
      <ElFormItem>
        <ElButton type="primary" :loading="loading" @click="handleUpdateRole">
          {{ t('el.common.role.update') }}
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
