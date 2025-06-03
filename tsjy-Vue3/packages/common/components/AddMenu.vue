<script setup>
import { useDrawer } from '../composables/drawer'
import { ElMessage, useLocale } from 'element-plus'
import { ref, watch } from 'vue'
import SelectRoute from './SelectRoute.vue'
import { addMenuApi } from '../apis/menu'

const props = defineProps(['modelValue', 'menu'])
const emits = defineEmits(['update:modelValue', 'finish'])

const { showDrawer, drawerTitle, drawerSize, openDrawer, closeDrawer } =
  useDrawer()

const { t } = useLocale()

watch(
  () => props.modelValue,
  (val) => {
    if (val) {
      drawerTitle.value = t('el.common.menu.addMenu')
      openDrawer('400')
    }
  },
)

watch(showDrawer, (val) => {
  emits('update:modelValue', val)
})

const formData = ref({
  label: '',
  isSubMenu: true,
  order: 0,
  parentId: null,
  route: '',
})

watch(
  () => props.menu,
  (val) => {
    if (val) {
      formData.value.parentId = val.id
      formData.value.isSubMenu = false
    } else {
      formData.value.parentId = null
      formData.value.isSubMenu = true
    }
  },
)

const loading = ref(false)

const handleAddMenu = async () => {
  if (loading.value) {
    return
  }

  loading.value = true
  try {
    await addMenuApi(formData.value)
    ElMessage({
      message: t('el.common.menu.addSuccessful'),
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
      message: t('el.common.menu.addFailed'),
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
      @keydown.enter.prevent="handleAddMenu"
    >
      <ElFormItem :label="t('el.common.menu.name')">
        <ElInput clearable v-model="formData.label"></ElInput>
      </ElFormItem>
      <ElFormItem :label="t('el.common.menu.route')">
        <div>
          <SelectRoute v-model="formData.route"></SelectRoute>
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
      <ElFormItem :label="t('el.common.menu.isSubMenu')">
        <ElSwitch v-model="formData.isSubMenu"></ElSwitch>
      </ElFormItem>
      <ElFormItem>
        <ElButton type="primary" @click="handleAddMenu">
          {{ t('el.common.menu.add') }}
        </ElButton>
      </ElFormItem>
    </ElForm>
  </ElDrawer>
</template>

<style scoped lang="scss"></style>
