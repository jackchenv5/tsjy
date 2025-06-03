import { http } from '#/common/apis/http'

export const getArchivedVariablesApi = (filter) => {
  return http.post('/api/VariableArchive/GetArchivedVariables', filter)
}

export const getUnarchivedVariablesApi = (filter) => {
  return http.post('/api/VariableArchive/GetUnarchivedVariables', filter)
}

export const addArchivedVariableApi = (guid, archiveMode) => {
  return http.post('/api/VariableArchive/AddArchivedVariable', null, {
    params: {
      guid,
      archiveMode,
    },
  })
}

export const updateArchivedVariableApi = (id, interval) => {
  return http.put(
    `/api/VariableArchive/UpdateArchivedVariable/${id}`,
    interval,
    {
      headers: {
        'Content-Type': 'application/json',
      },
    },
  )
}

export const deleteArchivedVariableApi = (id) => {
  return http.delete(`/api/VariableArchive/DeleteArchivedVariable/${id}`)
}
