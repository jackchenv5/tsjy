<script setup>
import { ElMessage, useLocale } from 'element-plus'
import { useDrawer } from '#/common/composables/drawer'
import { ref, watch } from 'vue'
import { addMaintainHistoryApi } from '../apis/maintainHistory'

// region 组件
const model = defineModel()
const { t } = useLocale()
const emits = defineEmits(['finished'])
const props = defineProps({
  planId: {
    type: Number,
    require: true,
  },
})
// endregion

// region 抽屉组件
const { showDrawer, drawerTitle, drawerSize, openDrawer, closeDrawer } =
  useDrawer()
watch(model, (val) => {
  if (val) {
    formData.value.maintainPlanId = props.planId
    drawerTitle.value = t('el.facilityMaintain.maintainPlans.partMaintain')
    openDrawer()
  }
})
watch(showDrawer, (val) => {
  model.value = val
})
// endregion

// region 处理维护数据
const formData = ref({
  maintainPlanId: 0,
  maintainReason: '',
  maintainContent: '',
})
const formRef = ref()
const formRules = ref({
  maintainReason: [
    {
      required: true,
      message: t('el.facilityMaintain.maintainPlans.requiredMsg'),
      trigger: 'blur',
    },
  ],
  maintainContent: [
    {
      required: true,
      message: t('el.facilityMaintain.maintainPlans.requiredMsg'),
      trigger: 'blur',
    },
  ],
})
const loadingAddMaintainHistory = ref(false)
const addMaintainHistory = async () => {
  loadingAddMaintainHistory.value = true
  try {
    await addMaintainHistoryApi(formData.value)
    ElMessage({
      message: t(
        'el.facilityMaintain.maintainPlans.saveMaintainContentSuccess',
      ),
      type: 'success',
    })
    formData.value = {
      maintainPlanId: 0,
      maintainReason: '',
      maintainContent: '',
    }
    emits('finished')
    closeDrawer()
  } catch (e) {
    ElMessage({
      message: t('el.facilityMaintain.maintainPlans.saveMaintainContentFail'),
      type: 'error',
    })
    console.error(e)
  } finally {
    loadingAddMaintainHistory.value = false
  }
}
const save = async () => {
  await formRef.value.validate(async (valid) => {
    if (valid) {
      await addMaintainHistory()
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
      <ElFormItem
        prop="maintainReason"
        :label="t('el.facilityMaintain.maintainPlans.maintainReason')"
      >
        <ElInput
          type="textarea"
          :autosize="{ minRows: 2, maxRows: 5 }"
          v-model="formData.maintainReason"
        >
        </ElInput>
      </ElFormItem>
      <ElFormItem
        prop="maintainContent"
        :label="t('el.facilityMaintain.maintainPlans.maintainContent')"
      >
        <ElInput
          type="textarea"
          :autosize="{ minRows: 2, maxRows: 5 }"
          v-model="formData.maintainContent"
        >
        </ElInput>
      </ElFormItem>
      <ElFormItem>
        <ElButton type="primary" @click="save">
          {{ t('el.facilityMaintain.maintainPlans.save') }}
        </ElButton>
      </ElFormItem>
    </ElForm>
  </ElDrawer>
</template>

<style scoped lang="scss"></style>
