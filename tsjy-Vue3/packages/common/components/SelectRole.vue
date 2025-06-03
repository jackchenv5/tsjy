<script setup>
import { useDrawer } from '../composables/drawer'
import { useLocale } from 'element-plus'
import { watch } from 'vue'
import { useGetRole } from '../composables/getRole'

// region 组件
const model = defineModel()
const { t } = useLocale()
const emits = defineEmits(['select'])
// endregion

// region 抽屉组件
const { showDrawer, drawerTitle, drawerSize, openDrawer, closeDrawer } =
  useDrawer()
watch(model, (val) => {
  if (val) {
    drawerTitle.value = t('el.common.role.selectRole')
    openDrawer('400')
  }
})
watch(showDrawer, (val) => {
  model.value = val
})
// endregion

// 角色数据
const {
  loading,
  data,
  pageIndex,
  pageSize,
  handlePageChange,
  handlePageSizeChange,
} = useGetRole()

const selectRole = (row) => {
  emits('select', JSON.parse(JSON.stringify(row)))
  closeDrawer()
}
// endregion
</script>

<template>
  <ElDrawer v-model="showDrawer" :title="drawerTitle" :size="drawerSize">
    <div class="select-user-root">
      <ElTable class="table" :data="data.items" stripe show-overflow-tooltip>
        <ElTableColumn
          :label="t('el.common.role.role')"
          min-width="160"
          prop="roleName"
        >
        </ElTableColumn>
        <ElTableColumn width="100">
          <template #default="{ row }">
            <ElButton
              type="primary"
              size="small"
              plain
              @click="selectRole(row)"
            >
              {{ t('el.common.role.select') }}
            </ElButton>
          </template>
        </ElTableColumn>
      </ElTable>
      <ElPagination
        layout="prev, pager, next, sizes, total"
        :total="data.total"
        :default-page-size="20"
        :pager-count="5"
        v-model:current-page="pageIndex"
        @update:current-page="handlePageChange"
        v-model:page-size="pageSize"
        :page-sizes="[20, 50, 100]"
        @update:page-size="handlePageSizeChange"
        :disabled="loading"
      >
      </ElPagination>
    </div>
  </ElDrawer>
</template>

<style scoped lang="scss">
.select-user-root {
  height: 100%;
  display: flex;
  flex-flow: column;
  grid-gap: 1rem;

  > .table {
    flex: 1;

    .full-name {
      font-size: var(--el-font-size-small);
      color: var(--el-text-color-secondary);
    }
  }
}
</style>
