<script setup>
import { useDrawer } from '../composables/drawer'
import { ElMessage, useLocale } from 'element-plus'
import { nextTick, ref, watch } from 'vue'
import { getRoleMenuApi, updateRoleMenuApi } from '../apis/roleMenu'
import { useAccessibleMenuStore } from '../stores/accessibleMenuStore'

const props = defineProps(['modelValue', 'role'])
const emits = defineEmits(['update:modelValue'])
const { showDrawer, drawerTitle, drawerSize, openDrawer, closeDrawer } =
  useDrawer()
const { t } = useLocale()
watch(
  () => props.modelValue,
  async (val) => {
    if (val) {
      drawerTitle.value = t('el.common.role.roleMenu')
      openDrawer('600')
      await getRoleMenu()
    }
  },
)

watch(showDrawer, (val) => {
  emits('update:modelValue', val)
})

const loading = ref()

const treeData = ref()
const treeRef = ref()
const checkStrictly = ref(false)

const setChecked = (menu) => {
  checkStrictly.value = true
  if (menu['hasMenu']) {
    treeRef.value.setChecked(menu['id'], true, false)
  } else {
    treeRef.value.setChecked(menu['id'], false, false)
  }
  if (menu['children']) {
    for (const child of menu['children']) {
      setChecked(child)
    }
  }
  checkStrictly.value = false
}

const getRoleMenu = async () => {
  if (loading.value) {
    return
  }

  loading.value = true
  try {
    const res = await getRoleMenuApi(props.role.id)
    treeData.value = res.data
    await nextTick(() => {
      for (const menu of treeData.value) {
        setChecked(menu)
      }
    })
  } catch (e) {
    console.error(e)
  } finally {
    loading.value = false
  }
}

const accessibleMenuStore = useAccessibleMenuStore()

const handleUpdateRoleMenu = async () => {
  if (loading.value) {
    return
  }

  const checkedMenus = treeRef.value.getCheckedNodes(false, true)
  const checkedIds = checkedMenus.map((menu) => menu['id'])

  loading.value = true
  try {
    await updateRoleMenuApi(props.role.id, checkedIds)
    await accessibleMenuStore.getAccessibleMenu()
    ElMessage({
      message: t('el.common.role.updateRoleMenuSuccessful'),
      type: 'success',
    })
    closeDrawer()
  } catch (e) {
    ElMessage({
      message: t('el.common.role.updateRoleMenuFailed'),
      type: 'error',
    })
    console.error(e)
  } finally {
    loading.value = false
  }
}
</script>

<template>
  <ElDrawer v-model="showDrawer" :title="drawerTitle" :size="drawerSize">
    <div class="container">
      <div class="role-name">
        <span>{{ t('el.common.role.roleColon') }}</span>
        <span>{{ role['roleName'] }}</span>
      </div>
      <div class="tree-container">
        <ElTree
          ref="treeRef"
          :data="treeData"
          show-checkbox
          node-key="id"
          default-expand-all
          :default-checked-keys="[]"
          :check-strictly="checkStrictly"
          style="margin: 1rem 0"
        >
          <template #default="{ data }">
            <div class="node">
              <div>{{ data.label }}</div>
              <div>
                <el-tag v-if="data.route" size="small" effect="dark">
                  {{ data.route }}
                </el-tag>
              </div>
            </div>
          </template>
        </ElTree>
      </div>
      <div class="footer">
        <ElButton type="primary" @click="handleUpdateRoleMenu">
          {{ t('el.common.role.update') }}
        </ElButton>
      </div>
    </div>
  </ElDrawer>
</template>

<style scoped lang="scss">
.container {
  height: 100%;
  overflow: auto;
  display: flex;
  flex-flow: column;

  > .role-name {
    border-left: 4px solid var(--el-color-primary);
    padding: 1rem;
    background-color: var(--el-color-primary-light-9);
  }

  > .tree-container {
    flex: 1;
    overflow: auto;
    border-top: 1px solid var(--el-border-color);
    border-bottom: 1px solid var(--el-border-color);
    margin-top: 1rem;

    .node {
      display: flex;
      flex-flow: row;
      flex: 1;

      > div {
        flex: 1;
      }
    }
  }

  > .footer {
    margin-top: 1rem;
  }
}
</style>
