<script setup>
import { useDrawer } from '#/common/composables/drawer'
import { ElMessage, useLocale } from 'element-plus'
import { ref, watch } from 'vue'
import { addMaintainPlanApi } from '../apis/maintainPlan'

// region 组件
const model = defineModel()
const { t } = useLocale()
const emits = defineEmits(['finished'])
const props = defineProps({
  facilityId: {
    type: Number,
    require: true,
  },
  partId: {
    type: String,
    require: true,
  },
  maintainCycle: {
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
    planFormData.value = {
      facilityId: props.facilityId,
      facilityPartId: props.partId,
      maintainCycle: props.maintainCycle,
    }
    if (props.maintainCycle < 1) {
      planFormData.value.maintainCycle = 1
    }

    drawerTitle.value = t('el.facilityMaintain.maintainPlan.addMaintainPlan')
    openDrawer()
  }
})
watch(showDrawer, (val) => {
  model.value = val
})
// endregion

// region 维护计划数据
const planFormData = ref({
  facilityId: 0,
  facilityPartId: '',
  maintainCycle: 1,
})
const loadingAddMaintainPlan = ref(false)
const addMaintainPlan = async () => {
  loadingAddMaintainPlan.value = true
  try {
    await addMaintainPlanApi(planFormData.value)
    ElMessage({
      message: t('el.facilityMaintain.maintainPlan.addSuccess'),
      type: 'success',
    })
    closeDrawer()
    emits('finished')
  } catch (e) {
    ElMessage({
      message: t('el.facilityMaintain.maintainPlan.addFailed'),
      type: 'error',
    })
    console.error(e)
  } finally {
    loadingAddMaintainPlan.value = false
  }
}

// endregion
</script>

<template>
  <ElDrawer v-model="model" :title="drawerTitle" :size="drawerSize">
    <ElForm v-model="planFormData" label-position="top">
      <ElFormItem
        :label="t('el.facilityMaintain.maintainPlan.maintainCycleWithUnit')"
        prop="maintainCycle"
      >
        <ElInputNumber v-model="planFormData.maintainCycle" :min="1">
        </ElInputNumber>
      </ElFormItem>
      <ElFormItem>
        <ElButton type="primary" @click="addMaintainPlan">
          {{ t('el.facilityMaintain.maintainPlan.add') }}
        </ElButton>
      </ElFormItem>
    </ElForm>
  </ElDrawer>
</template>

<style scoped lang="scss"></style>
