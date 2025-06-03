<script setup>
import { useDrawer } from '#/common/composables/drawer'
import { ref, watch } from 'vue'
import { useLocale } from 'element-plus'

// region 组件
const model = defineModel()
const emits = defineEmits(['finished'])
const props = defineProps(['title', 'confirmText'])
const { t } = useLocale()
// endregion

// region 抽屉组件
const internalConfirmText = ref()
const { showDrawer, drawerTitle, drawerSize, openDrawer, closeDrawer } =
  useDrawer()
watch(model, (val) => {
  if (val) {
    if (props.title) {
      drawerTitle.value = props.title
    } else {
      drawerTitle.value = t('el.variable.unbinding')
    }
    if (props.confirmText) {
      internalConfirmText.value = props.confirmText
    } else {
      internalConfirmText.value = t('el.variable.unbindingConfirm')
    }
    openDrawer()
  }
})
watch(showDrawer, (val) => {
  model.value = val
})
// endregion

// region 解绑
const onUnbindingClick = () => {
  closeDrawer()
  emits('finished')
}
// endregion
</script>

<template>
  <ElDrawer v-model="showDrawer" :title="drawerTitle" :size="drawerSize">
    <ElForm label-position="top">
      <ElFormItem>
        <template #label>
          <p class="confirm-text">
            {{ internalConfirmText }}
          </p>
        </template>
        <ElButton type="danger" @click="onUnbindingClick">
          {{ t('el.variable.unbinding') }}
        </ElButton>
      </ElFormItem>
    </ElForm>
  </ElDrawer>
</template>

<style scoped lang="scss"></style>
