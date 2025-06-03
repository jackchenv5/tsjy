<script setup>
import { useDrawer } from '../composables/drawer'
import { ElMessage, useLocale } from 'element-plus'
import { ref, watch } from 'vue'
import { deleteMenuApi } from '../apis/menu'
import SelectRoute from './SelectRoute.vue'

const props = defineProps(['modelValue', 'menu'])
const emits = defineEmits(['update:modelValue', 'finish'])

const { showDrawer, drawerTitle, drawerSize, openDrawer, closeDrawer } =
  useDrawer()

const { t } = useLocale()

watch(
  () => props.modelValue,
  (val) => {
    if (val) {
      drawerTitle.value = t('el.common.menu.deleteMenu')
      openDrawer('400')
    }
  },
)

watch(showDrawer, (val) => {
  emits('update:modelValue', val)
})

const formData = ref({
  label: '',
  isSubMenu: false,
  order: 0,
  parentId: null,
  route: '',
})

watch(
  () => props.menu,
  (val) => {
    if (val) {
      formData.value.label = val.label
      formData.value.isSubMenu = val.isSubMenu
      formData.value.order = val.order
      formData.value.parentId = val.parentId
      formData.value.route = val.route
    }
  },
)

const loading = ref(false)

const handleDeleteMenu = async () => {
  if (loading.value) {
    return
  }

  loading.value = true
  try {
    await deleteMenuApi(props.menu.id)
    ElMessage({
      message: t('el.common.menu.deleteSuccessful'),
      type: 'success',
    })
    formData.value = {
      label: '',
      isSubMenu: false,
      order: 0,
      parentId: null,
      route: '',
    }
    closeDrawer()
    emits('finish')
  } catch (e) {
    ElMessage({
      message: t('el.common.menu.deleteFailed'),
      type: 'error',
    })
    console.error(e)
  } finally {
    loading.value = false
  }
}
</script>

<template>
  <ElDrawer
    v-model="showDrawer"
    :title="drawerTitle"
    :size="drawerSize"
    destroy-on-close
  >
    <ElForm
      v-model="formData"
      label-position="top"
      @keydown.enter.prevent="handleDeleteMenu"
    >
      <ElFormItem :label="t('el.common.menu.name')" disabled>
        <ElInput clearable v-model="formData.label" disabled></ElInput>
      </ElFormItem>
      <ElFormItem :label="t('el.common.menu.route')">
        <div>
          <SelectRoute v-model="formData.route" disabled></SelectRoute>
          <div
            style="
              font-size: 1.2rem;
              color: var(--el-color-danger);
              line-height: 1.8rem;
            "
          >
            {{ t('el.common.menu.routeTip') }}
          </div>
        </div>
      </ElFormItem>
      <ElFormItem :label="t('el.common.menu.isSubMenu')" disabled>
        <ElSwitch v-model="formData.isSubMenu" disabled></ElSwitch>
      </ElFormItem>
      <ElFormItem>
        <template #label>
          <p class="confirm-text">
            {{ t('el.common.menu.deleteConfirm') }}
          </p>
        </template>
        <ElButton type="danger" @click="handleDeleteMenu">
          {{ t('el.common.menu.delete') }}
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
