<script setup>
import { useDrawer } from '#/common/composables/drawer'
import { ElMessage, useLocale } from 'element-plus'
import { ref, watch } from 'vue'
import { addFacilityApi } from '../apis/facility'

// region 组件
const model = defineModel()
const { t } = useLocale()
const emits = defineEmits(['finished'])
// endregion

// region 抽屉组件
const { showDrawer, drawerTitle, drawerSize, openDrawer, closeDrawer } =
  useDrawer()
watch(model, (val) => {
  if (val) {
    drawerTitle.value = t('el.facility.setting.addFacility')
    openDrawer()
  }
})
watch(showDrawer, (val) => {
  model.value = val
})
// endregion

// region 处理表单
const formData = ref({
  name: '',
  serialNumber: '',
  isEnabled: false,
  description: '',
})
const formRef = ref()
const formRules = ref({
  name: [
    {
      required: true,
      message: t('el.facility.setting.requiredMsg'),
      trigger: 'blur',
    },
  ],
})
const loadingAddFacility = ref(false)
const addFacility = async () => {
  loadingAddFacility.value = true
  try {
    await addFacilityApi(formData.value)
    ElMessage({
      message: t('el.facility.setting.addSuccessful'),
      type: 'success',
    })
    formData.value = {
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
      message: t('el.facility.setting.addFailed'),
      type: 'error',
    })
  } finally {
    loadingAddFacility.value = false
  }
}
const onAdd = async () => {
  await formRef.value.validate(async (valid) => {
    if (valid) {
      await addFacility()
    }
  })
}
// endregion
</script>

<template>
  <ElDrawer v-model="showDrawer" :title="drawerTitle" :size="drawerSize">
    <ElForm
      :model="formData"
      label-position="top"
      ref="formRef"
      :rules="formRules"
    >
      <ElFormItem prop="name" :label="t('el.facility.setting.name')" required>
        <ElInput v-model="formData.name"></ElInput>
      </ElFormItem>
      <ElFormItem
        prop="serialNumber"
        :label="t('el.facility.setting.serialNumber')"
      >
        <ElInput v-model="formData.serialNumber"></ElInput>
      </ElFormItem>
      <ElFormItem prop="isEnabled" :label="t('el.facility.setting.isEnabled')">
        <ElSwitch v-model="formData.isEnabled"></ElSwitch>
      </ElFormItem>
      <ElFormItem
        prop="description"
        :label="t('el.facility.setting.description')"
      >
        <ElInput v-model="formData.description"></ElInput>
      </ElFormItem>
      <ElFormItem>
        <ElButton type="primary" @click="onAdd" :loading="loadingAddFacility">
          {{ t('el.facility.setting.add') }}
        </ElButton>
      </ElFormItem>
    </ElForm>
  </ElDrawer>
</template>

<style scoped lang="scss"></style>
