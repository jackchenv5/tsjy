<script setup>
import { useDrawer } from '../composables/drawer'
import { ElMessage, useLocale } from 'element-plus'
import { ref, watch } from 'vue'
import { useValidateRoleName } from '../composables/validateRoleName'
import { addRoleApi } from '../apis/role'

const props = defineProps(['modelValue'])
const emits = defineEmits(['update:modelValue', 'finish'])

const { showDrawer, drawerTitle, drawerSize, openDrawer, closeDrawer } =
  useDrawer()
const { t } = useLocale()

watch(
  () => props.modelValue,
  (val) => {
    if (val) {
      drawerTitle.value = t('el.common.role.addRole')
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

const loading = ref(false)

const { validRoleName, validateRoleName } = useValidateRoleName()

const handleAddRole = async () => {
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
    await addRoleApi(formData.value)
    ElMessage({
      message: t('el.common.role.addSuccessful'),
      type: 'success',
    })
    closeDrawer()
    formData.value = {
      roleName: '',
    }
    emits('finish')
  } catch (e) {
    ElMessage({
      message: t('el.common.role.addFailed'),
      type: 'error',
    })
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
      @keydown.enter.prevent="handleAddRole"
      v-loading="loading"
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
        <ElButton type="primary" :loading="loading" @click="handleAddRole">
          {{ t('el.common.role.add') }}
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
