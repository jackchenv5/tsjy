<script setup>
import ContentLayout from '../layouts/ContentLayout.vue'
import { ElMessage, useLocale } from 'element-plus'
import { onMounted, ref } from 'vue'
import { getMenuApi, updateMenusApi } from '../apis/menu'
import AddMenu from '../components/AddMenu.vue'
import UpdateMenu from '../components/UpdateMenu.vue'
import DeleteMenu from '../components/DeleteMenu.vue'
import { useAccessibleMenuStore } from '../stores/accessibleMenuStore'

const { t } = useLocale()

const loading = ref(false)
const menus = ref()

const getMenu = async () => {
  if (loading.value) {
    return
  }

  loading.value = true

  try {
    const res = await getMenuApi()
    menus.value = res.data
  } catch (e) {
    console.error(e)
  } finally {
    loading.value = false
  }
}

onMounted(async () => {
  await getMenu()
})

const allowDrop = (draggingNode, dropNode, type) => {
  if (dropNode.data['isSubMenu']) {
    return true
  } else {
    if (type !== 'inner') {
      return true
    }
  }
}

const currentMenu = ref()

const showAddMenu = ref(false)
const handleAddMenu = (menu) => {
  if (menu) {
    currentMenu.value = JSON.parse(JSON.stringify(menu))
  } else {
    currentMenu.value = null
  }
  showAddMenu.value = true
}

const showUpdateMenu = ref(false)
const handleUpdateMenu = (menu) => {
  currentMenu.value = JSON.parse(JSON.stringify(menu))
  showUpdateMenu.value = true
}

const showDeleteMenu = ref(false)
const handleDeleteMenu = (menu) => {
  currentMenu.value = JSON.parse(JSON.stringify(menu))
  showDeleteMenu.value = true
}

const accessibleMenuStore = useAccessibleMenuStore()
const handleEditFinish = async () => {
  await getMenu()
  await accessibleMenuStore.getAccessibleMenu()
}

let order = 1
let editingMenus = []

const setMenuOrder = (menu, parentId = null) => {
  menu.order = order++
  menu.parentId = parentId
  editingMenus.push(menu)
  if (menu.children) {
    for (let child of menu.children) {
      setMenuOrder(child, menu.id)
    }
  }
}

const handleSaveMenuOrder = async () => {
  if (loading.value) {
    return
  }

  order = 1
  editingMenus = []
  for (let menu of menus.value) {
    setMenuOrder(menu)
  }

  loading.value = true
  try {
    await updateMenusApi(editingMenus)
    ElMessage({
      message: t('el.common.menu.updateOrderSuccessful'),
      type: 'success',
    })
    await handleEditFinish()
  } catch (e) {
    ElMessage({
      message: t('el.common.menu.updateOrderFailed'),
      type: 'error',
    })
    console.error(e)
  } finally {
    loading.value = false
  }
}
</script>

<template>
  <ContentLayout>
    <template #header>
      {{ t('el.common.menu.menuSetting') }}
    </template>
    <template #default>
      <div>
        <ElButton type="primary" plain @click="handleAddMenu(null)">
          {{ t('el.common.menu.addRootMenu') }}
        </ElButton>
      </div>
      <div class="data-container">
        <div class="tree-container">
          <ElTree
            :data="menus"
            draggable
            :allow-drop="allowDrop"
            style="margin: 1rem 0"
            default-expand-all
            :expand-on-click-node="false"
            node-key="id"
          >
            <template #default="{ data }">
              <div class="tree">
                <div class="left">
                  <div class="label">
                    <ElTooltip :content="data.label" :show-after="500">
                      <ElText truncated>{{ data.label }}</ElText>
                    </ElTooltip>
                  </div>
                  <div class="route">
                    <ElTooltip :content="data.route" :show-after="500">
                      <ElText truncated>{{ data.route }}</ElText>
                    </ElTooltip>
                  </div>
                </div>
                <div class="right">
                  <ElButton
                    type="primary"
                    link
                    size="small"
                    @click="handleUpdateMenu(data)"
                  >
                    {{ t('el.common.menu.update') }}
                  </ElButton>
                  <ElButton
                    type="danger"
                    link
                    size="small"
                    @click="handleDeleteMenu(data)"
                  >
                    {{ t('el.common.menu.delete') }}
                  </ElButton>
                  <ElButton
                    type="primary"
                    size="small"
                    link
                    v-if="data.isSubMenu"
                    @click="handleAddMenu(data)"
                  >
                    {{ t('el.common.menu.addMenu') }}
                  </ElButton>
                </div>
              </div>
            </template>
          </ElTree>
        </div>
        <div class="footer">
          <ElButton type="primary" @click="handleSaveMenuOrder">
            {{ t('el.common.menu.updateMenuOrder') }}
          </ElButton>
        </div>
      </div>
    </template>
  </ContentLayout>

  <AddMenu :menu="currentMenu" v-model="showAddMenu" @finish="handleEditFinish">
  </AddMenu>
  <UpdateMenu
    :menu="currentMenu"
    v-model="showUpdateMenu"
    @finish="handleEditFinish"
  >
  </UpdateMenu>
  <DeleteMenu
    :menu="currentMenu"
    v-model="showDeleteMenu"
    @finish="handleEditFinish"
  >
  </DeleteMenu>
</template>

<style scoped lang="scss">
.data-container {
  margin-top: 1rem;
  display: flex;
  height: 100%;
  flex-flow: column;
  overflow: auto;

  > .tree-container {
    flex: 1;
    overflow: auto;
    border-top: 1px solid var(--el-border-color);
    border-bottom: 1px solid var(--el-border-color);

    .tree {
      display: flex;
      flex-flow: row nowrap;
      flex: 1;
      overflow: hidden;
      grid-gap: 2rem;
      min-width: 60rem;

      > .left {
        flex: 1;
        display: flex;
        overflow: hidden;
        grid-gap: 2rem;

        > .label {
          width: 16rem;
          overflow: hidden;
        }

        > .route {
          flex: 1;
          overflow: hidden;
        }
      }

      > .right {
        min-width: 16rem;
      }
    }
  }

  > .footer {
    margin-top: 1rem;
  }
}
</style>
