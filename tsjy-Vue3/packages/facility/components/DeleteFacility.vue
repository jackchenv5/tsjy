<script setup>
import { ElMessage, useLocale } from 'element-plus'
import { useDrawer } from '#/common/composables/drawer'
import { ref, watch } from 'vue'
import { deleteFacilityApi } from '../apis/facility'

// region 组件
const model = defineModel()
const { t } = useLocale()
const props = defineProps(['facility'])
const emits = defineEmits(['finished'])
// endregion

// region 抽屉组件
const { showDrawer, drawerTitle, drawerSize, openDrawer, closeDrawer } =
  useDrawer()
watch(model, (val) => {
  if (val) {
    drawerTitle.value = t('el.facility.setting.deleteFacility')
    openDrawer()
  }
})
watch(showDrawer, (val) => {
  model.value = val
})
// endregion

// region 处理表单
const formData = ref({
  id: 0,
  name: '',
  serialNumber: '',
  isEnabled: false,
  description: '',
})
watch(
  () => props.facility,
  (value) => {
    if (value) {
      formData.value.id = value.id
      formData.value.name = value.name
      formData.value.serialNumber = value.serialNumber
      formData.value.isEnabled = value.isEnabled
      formData.value.description = value.description
    }
  },
)
const loadingDeleteFacility = ref(false)
const deleteFacility = async () => {
  loadingDeleteFacility.value = true
  try {
    await deleteFacilityApi(formData.value.id)
    ElMessage({
      message: t('el.facility.setting.deleteSuccessful'),
      type: 'success',
    })
    formData.value = {
      id: 0,
      name: '',
      serialNumber: '',
      isEnabled: false,
      description: '',
    }
    emits('finished')
    closeDrawer()
  } catch (e) {
    console.error(e)
    ElMessage({
      message: t('el.facility.setting.deleteFailed'),
      type: 'error',
    })
  } finally {
    loadingDeleteFacility.value = false
  }
}
const onDelete = async () => {
  await deleteFacility()
}
// endregion
</script>

<template>
  <ElDrawer v-model="showDrawer" :title="drawerTitle" :size="drawerSize">
    <ElForm :model="formData" label-position="top" ref="formRef">
      <ElFormItem prop="name" :label="t('el.facility.setting.name')" required>
        <ElInput v-model="formData.name" disabled></ElInput>
      </ElFormItem>
      <ElFormItem
        prop="serialNumber"
        :label="t('el.facility.setting.serialNumber')"
      >
        <ElInput v-model="formData.serialNumber" disabled></ElInput>
      </ElFormItem>
      <ElFormItem prop="isEnabled" :label="t('el.facility.setting.isEnabled')">
        <ElSwitch v-model="formData.isEnabled" disabled></ElSwitch>
      </ElFormItem>
      <ElFormItem
        prop="description"
        :label="t('el.facility.setting.description')"
      >
        <ElInput v-model="formData.description" disabled></ElInput>
      </ElFormItem>
      <ElFormItem>
        <template #label>
          <p class="confirm-text">
            {{ t('el.facility.setting.deleteConfirm') }}
          </p>
        </template>
        <ElButton
          type="danger"
          @click="onDelete"
          :loading="loadingDeleteFacility"
        >
          {{ t('el.facility.setting.delete') }}
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
