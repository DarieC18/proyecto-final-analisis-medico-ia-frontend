<template>
  <div>
    <LoadingState v-if="loading" label="Cargando expediente…" />

    <AppAlert v-if="error" variant="danger" :message="error" dismissible class="mb-3" @close="error = ''" />

    <template v-if="cita">
      <PageHeader
        :title="`Expediente Clínico: ${patientName}`"
        :icon="IconRecord"
        back-to="/citas"
      >
        <template #meta>
          <p class="d-flex align-items-center gap-2 mt-1 mb-0">
            <small class="text-app-muted">Cita #{{ cita.id }}</small>
            <StatusBadge :text="translateStatus(cita.status)" :variant="statusVariant(cita.status)" />
          </p>
        </template>
      </PageHeader>

      <div class="row g-3 mb-4">
        <div class="col-md-3">
          <BaseCard tone="muted" padding="sm" class="h-100">
            <DataField label="Fecha" :value="formatDateTime(cita.appointmentDate)" :icon="IconAppointment" />
          </BaseCard>
        </div>
        <div class="col-md-3">
          <BaseCard tone="muted" padding="sm" class="h-100">
            <DataField label="Motivo" :value="cita.reason" :icon="IconNotes" />
          </BaseCard>
        </div>
        <div class="col-md-3">
          <BaseCard tone="muted" padding="sm" class="h-100">
            <DataField label="Paciente" :value="patientName" :icon="IconUser" />
          </BaseCard>
        </div>
        <div class="col-md-3">
          <BaseCard tone="muted" padding="sm" class="h-100">
            <label for="c-estado" class="label-eyebrow d-block mb-1">Estado</label>
            <select id="c-estado" v-model="nuevoEstado" class="form-select form-select-sm" @change="cambiarEstado">
              <option value="Pending">Pendiente</option>
              <option value="InProgress">En Progreso</option>
              <option value="Completed">Completada</option>
              <option value="Cancelled">Cancelada</option>
            </select>
          </BaseCard>
        </div>
      </div>

      <BaseCard flush padding="none" class="mb-3">
        <TabNav v-model="tabActual" :tabs="tabsVisibles" />
      </BaseCard>

      <div class="card shadow-sm">
        <div class="card-body p-4">

          <!-- INFO TAB -->
          <div v-if="tabActual === 'info'" class="animation-fade">
            <h5 class="fw-bold text-dark mb-4">Notas y Registro Clínico</h5>

            <div class="mb-4">
              <label class="form-label text-muted small fw-bold text-uppercase">Notas de la Cita</label>
              <textarea v-model="notasEdit" class="form-control bg-light border-0 p-4 rounded-4 mb-3" rows="4" placeholder="Redacte las notas de la consulta aquí..."></textarea>
              <div class="d-flex justify-content-end">
                <button @click="guardarNotas" class="btn btn-primary px-4 py-2" :disabled="!notasEdit || guardandoNotas">
                  <span v-if="guardandoNotas" class="spinner-border spinner-border-sm me-2"></span>
                  Guardar Notas
                </button>
              </div>
              <AppAlert v-if="notasGuardadas" variant="success" message="Notas guardadas exitosamente." class="mt-3" />
            </div>

            <hr class="my-4">

            <h5 class="fw-bold text-dark mb-4">Registros Médicos</h5>
            <div v-if="cita.medicalRecords?.length">
              <div v-for="r in cita.medicalRecords" :key="r.id" class="card bg-light border-0 p-4 rounded-4 mb-3">
                <div class="d-flex justify-content-between mb-2">
                  <small class="text-muted fw-bold">{{ formatDate(r.createdAt) }}</small>
                  <small class="text-muted">Dr. #{{ r.createdByUserId }}</small>
                </div>
                <p class="mb-1"><strong>Diagnóstico Inicial:</strong> {{ r.diagnosisInitial || 'N/A' }}</p>
                <p class="mb-1"><strong>Notas:</strong> {{ r.notes || 'N/A' }}</p>
                <p v-if="r.antecedentes" class="mb-1"><strong>Antecedentes:</strong> {{ r.antecedentes }}</p>
                <p v-if="r.observacionesConsulta" class="mb-0"><strong>Observaciones:</strong> {{ r.observacionesConsulta }}</p>
              </div>
            </div>
            <div v-else class="text-muted text-center py-4">
              <p>No hay registros médicos para esta cita.</p>
            </div>
          </div>

          <!-- SINTOMAS TAB -->
          <div v-if="tabActual === 'sintomas'" class="animation-fade">
            <div class="d-flex justify-content-between align-items-center mb-4">
              <h5 class="fw-bold text-dark mb-0">Registro de Síntomas</h5>
              <button v-if="!auth.hasRole('Nurse')" @click="toggleFormSintoma" class="btn btn-primary btn-sm px-3 shadow-sm">
                {{ showSintomaForm ? 'Cancelar' : '+ Agregar Síntoma' }}
              </button>
            </div>

            <AppAlert v-if="sintomaError" variant="danger" :message="sintomaError" class="mb-3" />

            <FormSection v-if="showSintomaForm" title="Nuevo Síntoma" :icon="IconSymptoms" class="mb-4">
                <div class="row g-3">
                  <div class="col-md-4">
                    <label class="form-label fw-medium">Síntoma</label>
                    <input v-model="sintomaForm.name" type="text" class="form-control" placeholder="Ej: Fiebre">
                  </div>
                  <div class="col-md-3">
                    <label class="form-label fw-medium">Severidad</label>
                    <select v-model="sintomaForm.severity" class="form-select">
                      <option value="Leve">Leve</option>
                      <option value="Moderado">Moderado</option>
                      <option value="Severo">Severo</option>
                    </select>
                  </div>
                  <div class="col-md-3">
                    <label class="form-label fw-medium">Inicio</label>
                    <input v-model="sintomaForm.startedAt" type="date" class="form-control">
                  </div>
                  <div class="col-md-2 d-flex align-items-end">
                    <button @click="guardarSintoma" class="btn btn-success w-100 rounded-pill" :disabled="guardandoSintoma">
                      <span v-if="guardandoSintoma" class="spinner-border spinner-border-sm"></span>
                      <span v-else>+ Agregar</span>
                    </button>
                  </div>
                </div>
                <div class="mt-3">
                  <label class="form-label fw-medium">Notas</label>
                  <textarea v-model="sintomaForm.notes" class="form-control" rows="2" placeholder="Notas adicionales..."></textarea>
                </div>
              
            </FormSection>

            <EmptyState
              v-if="sintomas.length === 0"
              size="sm"
              :icon="IconSymptoms"
              title="Sin síntomas registrados"
            />
            <div v-else class="row g-3">
              <div class="col-md-6" v-for="(s, idx) in sintomas" :key="s.id || idx">
                <div class="sintoma" :class="`sintoma--${severityVariant(s.severity)}`">
                  <div class="d-flex justify-content-between align-items-start gap-2 mb-2">
                    <h6 class="sintoma__nombre">{{ s.name }}</h6>
                    <StatusBadge :text="s.severity" :variant="severityVariant(s.severity)" />
                  </div>
                  <DataField label="Inicio" :value="formatDate(s.startedAt)" />
                  <p class="sintoma__notas">{{ s.notes || 'Sin notas adicionales.' }}</p>
                  <div v-if="!auth.hasRole('Nurse')" class="d-flex justify-content-end mt-2">
                    <AppButton variant="soft" size="sm" :icon="IconEdit" @click="editarSintoma(s)">
                      Editar
                    </AppButton>
                  </div>
                </div>
              </div>
            </div>

            <FormSection v-if="editSintomaId" title="Editar Síntoma" :icon="IconEdit" class="mt-4">
                <div class="row g-3">
                  <div class="col-md-4">
                    <label class="form-label fw-medium">Síntoma</label>
                    <input v-model="editSintomaForm.name" type="text" class="form-control">
                  </div>
                  <div class="col-md-3">
                    <label class="form-label fw-medium">Severidad</label>
                    <select v-model="editSintomaForm.severity" class="form-select">
                      <option value="Leve">Leve</option>
                      <option value="Moderado">Moderado</option>
                      <option value="Severo">Severo</option>
                    </select>
                  </div>
                  <div class="col-md-3">
                    <label class="form-label fw-medium">Inicio</label>
                    <input v-model="editSintomaForm.startedAt" type="date" class="form-control">
                  </div>
                  <div class="col-md-2 d-flex align-items-end gap-2">
                    <button @click="actualizarSintoma" class="btn btn-success w-100 rounded-pill" :disabled="guardandoSintoma">
                      <span v-if="guardandoSintoma" class="spinner-border spinner-border-sm"></span>
                      <span v-else>💾 Guardar</span>
                    </button>
                    <button @click="cancelarEditSintoma" class="btn btn-outline-secondary rounded-pill w-100">Cancelar</button>
                  </div>
                </div>
                <div class="mt-3">
                  <label class="form-label fw-medium">Notas</label>
                  <textarea v-model="editSintomaForm.notes" class="form-control" rows="2"></textarea>
                </div>
              
            </FormSection>

            <ConfirmDialog
              :visible="deleteSintomaDialog"
              title="Eliminar Síntoma"
              message="¿Está seguro que desea eliminar este síntoma? Esta acción no se puede deshacer."
              confirmText="Eliminar"
              :danger="true"
              @confirm="eliminarSintoma"
              @cancel="deleteSintomaDialog = false"
            />
          </div>

          <!-- SIGNOS VITALES TAB -->
          <div v-if="tabActual === 'signos'" class="animation-fade">
            <div class="d-flex justify-content-between align-items-center mb-4">
              <div>
                <h5 class="fw-bold text-dark mb-0">Mediciones Clínicas</h5>
                <p class="text-muted small mb-0" v-if="signos.length">Última toma: {{ formatDateTime(signos[signos.length - 1]?.measuredAt) }}</p>
              </div>
              <button v-if="!auth.hasRole('Nurse')" @click="toggleFormSignos" class="btn btn-primary btn-sm px-3 shadow-sm">
                {{ showSignosForm ? 'Cancelar' : '+ Registrar Medición' }}
              </button>
            </div>

            <AppAlert v-if="signosError" variant="danger" :message="signosError" class="mb-3" />

            <FormSection v-if="showSignosForm" title="Nueva Medición" :icon="IconVitals" class="mb-4">
                <div class="row g-3">
                  <div class="col-md-4">
                    <label class="form-label fw-medium">🌡️ Temperatura (°C)</label>
                    <input v-model.number="signosForm.temperature" type="number" step="0.1" class="form-control" placeholder="36.5">
                  </div>
                  <div class="col-md-4">
                    <label class="form-label fw-medium">💓 Frec. Cardíaca (lpm)</label>
                    <input v-model.number="signosForm.heartRate" type="number" class="form-control" placeholder="72">
                  </div>
                  <div class="col-md-4">
                    <label class="form-label fw-medium">🩸 Presión Sistólica</label>
                    <input v-model.number="signosForm.systolicPressure" type="number" class="form-control" placeholder="120">
                  </div>
                  <div class="col-md-4">
                    <label class="form-label fw-medium">🩸 Presión Diastólica</label>
                    <input v-model.number="signosForm.diastolicPressure" type="number" class="form-control" placeholder="80">
                  </div>
                  <div class="col-md-4">
                    <label class="form-label fw-medium">🫁 Saturación O2 (%)</label>
                    <input v-model.number="signosForm.oxygenSaturation" type="number" step="0.1" class="form-control" placeholder="98">
                  </div>
                  <div class="col-md-4">
                    <label class="form-label fw-medium">🍬 Glucosa (mg/dL)</label>
                    <input v-model.number="signosForm.glucose" type="number" class="form-control" placeholder="95">
                  </div>
                </div>
                <div class="d-flex justify-content-end mt-4">
                  <button @click="guardarSignos" class="btn btn-success rounded-pill px-4 shadow-sm" :disabled="guardandoSignos">
                    <span v-if="guardandoSignos" class="spinner-border spinner-border-sm me-2"></span>
                    💾 Guardar Medición
                  </button>
                </div>
              
            </FormSection>

            <div v-if="signos.length === 0 && !showSignosForm" class="text-muted text-center py-4">
              <p>No hay signos vitales registrados para esta cita.</p>
            </div>

            <div v-if="signos.length" class="row g-4 text-center mt-2">
              <div class="col-md-4 col-sm-6">
                <div class="card bg-light border-0 shadow-sm p-4 rounded-4 h-100">
                  <div class="display-6 mb-2">🌡️</div>
                  <h6 class="text-muted fw-bold text-uppercase mb-1" style="font-size: 0.8rem;">Temperatura</h6>
                  <h3 class="fw-bold mb-0" :class="ultimoSigno.temperature >= 37.5 ? 'text-danger' : 'text-dark'">{{ ultimoSigno.temperature }} °C</h3>
                </div>
              </div>
              <div class="col-md-4 col-sm-6">
                <div class="card bg-light border-0 shadow-sm p-4 rounded-4 h-100">
                  <div class="display-6 mb-2">💓</div>
                  <h6 class="text-muted fw-bold text-uppercase mb-1" style="font-size: 0.8rem;">Frec. Cardíaca</h6>
                  <h3 class="fw-bold text-dark mb-0">{{ ultimoSigno.heartRate }} lpm</h3>
                </div>
              </div>
              <div class="col-md-4 col-sm-6">
                <div class="card bg-light border-0 shadow-sm p-4 rounded-4 h-100">
                  <div class="display-6 mb-2">🩸</div>
                  <h6 class="text-muted fw-bold text-uppercase mb-1" style="font-size: 0.8rem;">Presión Arterial</h6>
                  <h3 class="fw-bold text-dark mb-0">{{ ultimoSigno.systolicPressure }}/{{ ultimoSigno.diastolicPressure }}</h3>
                  <small class="text-muted fw-medium">mmHg</small>
                </div>
              </div>
              <div class="col-md-6 col-sm-6">
                <div class="card bg-light border-0 shadow-sm p-4 rounded-4 h-100">
                  <div class="display-6 mb-2">🫁</div>
                  <h6 class="text-muted fw-bold text-uppercase mb-1" style="font-size: 0.8rem;">Saturación O2</h6>
                  <h3 class="fw-bold mb-0" :class="ultimoSigno.oxygenSaturation < 95 ? 'text-warning' : 'text-dark'">{{ ultimoSigno.oxygenSaturation }} %</h3>
                </div>
              </div>
              <div class="col-md-6 col-sm-6">
                <div class="card bg-light border-0 shadow-sm p-4 rounded-4 h-100">
                  <div class="display-6 mb-2">🍬</div>
                  <h6 class="text-muted fw-bold text-uppercase mb-1" style="font-size: 0.8rem;">Glucosa</h6>
                  <h3 class="fw-bold text-dark mb-0">{{ ultimoSigno.glucose || 'N/A' }}</h3>
                  <small class="text-muted fw-medium">mg/dL</small>
                </div>
              </div>
            </div>

            <div v-if="signos.length > 1" class="mt-4">
              <h6 class="fw-bold text-muted mb-3">Mediciones Anteriores</h6>
              <div class="table-responsive">
                <table class="table table-sm table-hover align-middle mb-0">
                  <thead class="bg-light text-muted">
                    <tr>
                      <th class="py-2">Fecha</th>
                      <th class="py-2">Temp</th>
                      <th class="py-2">FC</th>
                      <th class="py-2">PA</th>
                      <th class="py-2">O2</th>
                      <th class="py-2">Glucosa</th>
                      <th class="py-2">Acción</th>
                    </tr>
                  </thead>
                  <tbody>
                    <tr v-for="s in signos.slice().reverse()" :key="s.id">
                      <td class="py-2 text-muted small">{{ formatDateTime(s.measuredAt) }}</td>
                      <td class="py-2">{{ s.temperature }}°C</td>
                      <td class="py-2">{{ s.heartRate }} lpm</td>
                      <td class="py-2">{{ s.systolicPressure }}/{{ s.diastolicPressure }}</td>
                      <td class="py-2">{{ s.oxygenSaturation }}%</td>
                      <td class="py-2">{{ s.glucose || '-' }}</td>
                      <td class="py-2">
                        <button @click="editarSigno(s)" class="btn btn-sm btn-light border text-warning px-2">Editar</button>
                      </td>
                    </tr>
                  </tbody>
                </table>
              </div>
            </div>

            <FormSection v-if="editSignoId" title="Editar Medición" :icon="IconEdit" class="mt-4">
                <div class="row g-3">
                  <div class="col-md-4">
                    <label class="form-label fw-medium">🌡️ Temperatura (°C)</label>
                    <input v-model.number="editSignoForm.temperature" type="number" step="0.1" class="form-control">
                  </div>
                  <div class="col-md-4">
                    <label class="form-label fw-medium">💓 Frec. Cardíaca (lpm)</label>
                    <input v-model.number="editSignoForm.heartRate" type="number" class="form-control">
                  </div>
                  <div class="col-md-4">
                    <label class="form-label fw-medium">🩸 Presión Sistólica</label>
                    <input v-model.number="editSignoForm.systolicPressure" type="number" class="form-control">
                  </div>
                  <div class="col-md-4">
                    <label class="form-label fw-medium">🩸 Presión Diastólica</label>
                    <input v-model.number="editSignoForm.diastolicPressure" type="number" class="form-control">
                  </div>
                  <div class="col-md-4">
                    <label class="form-label fw-medium">🫁 Saturación O2 (%)</label>
                    <input v-model.number="editSignoForm.oxygenSaturation" type="number" step="0.1" class="form-control">
                  </div>
                  <div class="col-md-4">
                    <label class="form-label fw-medium">🍬 Glucosa (mg/dL)</label>
                    <input v-model.number="editSignoForm.glucose" type="number" class="form-control">
                  </div>
                </div>
                <div class="d-flex justify-content-end gap-2 mt-4">
                  <button @click="cancelarEditSigno" class="btn btn-outline-secondary rounded-pill px-4">Cancelar</button>
                  <button @click="actualizarSigno" class="btn btn-success rounded-pill px-4 shadow-sm" :disabled="guardandoSignos">
                    <span v-if="guardandoSignos" class="spinner-border spinner-border-sm me-2"></span>
                    💾 Guardar Cambios
                  </button>
                </div>
              
            </FormSection>
          </div>

          <!-- HISTORIAL MÉDICO TAB -->
          <div v-if="tabActual === 'records'" class="animation-fade">
            <div class="d-flex justify-content-between align-items-center mb-4">
              <h5 class="fw-bold text-dark mb-0">Registros Médicos</h5>
              <button @click="toggleFormRecord" class="btn btn-primary btn-sm px-3 shadow-sm">
                {{ showRecordForm ? 'Cancelar' : '+ Nuevo Registro' }}
              </button>
            </div>

            <AppAlert v-if="recordError" variant="danger" :message="recordError" class="mb-3" />

            <FormSection v-if="showRecordForm" :title="`${editRecordId ? 'Editar' : 'Nuevo'} Registro Médico`" :icon="IconRecord" class="mb-4">
                <div class="row g-3">
                  <div class="col-md-6">
                    <label class="form-label fw-medium">🩺 Diagnóstico Inicial</label>
                    <input v-model="recordForm.diagnosisInitial" type="text" class="form-control" placeholder="Diagnóstico principal">
                  </div>
                  <div class="col-md-6">
                    <label class="form-label fw-medium">📋 Antecedentes</label>
                    <input v-model="recordForm.antecedentes" type="text" class="form-control" placeholder="Antecedentes del paciente">
                  </div>
                  <div class="col-12">
                    <label class="form-label fw-medium">📝 Notas</label>
                    <textarea v-model="recordForm.notes" class="form-control" rows="3" placeholder="Notas del registro médico"></textarea>
                  </div>
                  <div class="col-12">
                    <label class="form-label fw-medium">👁️ Observaciones de Consulta</label>
                    <textarea v-model="recordForm.observacionesConsulta" class="form-control" rows="2" placeholder="Observaciones adicionales"></textarea>
                  </div>
                </div>
                <div class="d-flex justify-content-end gap-2 mt-4">
                  <button type="button" @click="cancelarFormRecord" class="btn btn-outline-secondary rounded-pill px-4">Cancelar</button>
                  <button @click="guardarRecord" class="btn btn-success rounded-pill px-4 shadow-sm" :disabled="guardandoRecord">
                    <span v-if="guardandoRecord" class="spinner-border spinner-border-sm me-2"></span>
                    💾 {{ editRecordId ? 'Actualizar' : 'Guardar' }}
                  </button>
                </div>
              
            </FormSection>

            <div v-if="recordsMedicos.length === 0" class="text-muted text-center py-4">
              <p>No hay registros médicos para esta cita.</p>
            </div>
            <div v-for="r in recordsMedicos" :key="r.id" class="card bg-light border-0 p-4 rounded-4 mb-3">
              <div class="d-flex justify-content-between mb-2">
                <small class="text-muted fw-bold">{{ formatDate(r.createdAt) }}</small>
                <div class="d-flex gap-2">
                  <button @click="editarRecord(r)" class="btn btn-sm btn-light border text-warning px-3">Editar</button>
                </div>
              </div>
              <p class="mb-1"><strong>Diagnóstico Inicial:</strong> {{ r.diagnosisInitial || 'N/A' }}</p>
              <p class="mb-1"><strong>Notas:</strong> {{ r.notes || 'N/A' }}</p>
              <p v-if="r.antecedentes" class="mb-1"><strong>Antecedentes:</strong> {{ r.antecedentes }}</p>
              <p v-if="r.observacionesConsulta" class="mb-0"><strong>Observaciones:</strong> {{ r.observacionesConsulta }}</p>
            </div>
          </div>

          <!-- ALERTAS TAB -->
          <div v-if="tabActual === 'alertas'" class="animation-fade">
            <div class="d-flex justify-content-between align-items-center mb-4">
              <h5 class="fw-bold text-dark mb-0">Alertas de la Cita</h5>
            </div>
            <LoadingState v-if="cargandoAlertas" />
            <div v-else-if="alertas.length === 0" class="text-muted text-center py-4">
              <p>No hay alertas asociadas a esta cita.</p>
            </div>
            <div v-for="a in alertas" :key="a.id" class="card mb-3 border-0 shadow-sm rounded-4" :class="a.isResolved ? 'bg-light' : 'border-start border-4 border-danger'">
              <div class="card-body p-4">
                <div class="d-flex justify-content-between mb-2">
                  <h6 class="fw-bold mb-0">{{ a.alertType || 'Alerta' }}</h6>
                  <span class="badge rounded-pill px-3" :class="a.isResolved ? 'bg-success' : 'bg-danger'">
                    {{ a.isResolved ? 'Resuelta' : 'Activa' }}
                  </span>
                </div>
                <p class="text-muted mb-2" v-if="a.description">{{ a.description }}</p>
                <small class="text-muted">{{ formatDate(a.createdAt) }}</small>
              </div>
            </div>
          </div>

          <!-- RECOMENDACIONES TAB -->
          <div v-if="tabActual === 'recomendaciones'" class="animation-fade">
            <div class="d-flex justify-content-between align-items-center mb-4">
              <h5 class="fw-bold text-dark mb-0">Recomendaciones</h5>
            </div>
            <LoadingState v-if="cargandoRecomendaciones" />
            <div v-else-if="recomendaciones.length === 0" class="text-muted text-center py-4">
              <p>No hay recomendaciones para esta cita.</p>
            </div>
            <div v-for="r in recomendaciones" :key="r.id" class="card bg-light border-0 border-start border-4 border-info p-4 rounded-4 mb-3">
              <h6 class="fw-bold mb-2">{{ r.title || 'Recomendación' }}</h6>
              <p class="text-muted mb-2" v-if="r.description">{{ r.description }}</p>
              <small class="text-muted">{{ formatDate(r.createdAt) }}</small>
            </div>
          </div>

          <!-- DOCUMENTOS TAB -->
          <div v-if="tabActual === 'documentos'" class="animation-fade">
            <div class="d-flex justify-content-between align-items-center mb-4">
              <h5 class="fw-bold mb-0">Documentos</h5>
              <AppButton
                variant="primary"
                size="sm"
                :icon="showDocUpload ? IconClose : IconUpload"
                @click="showDocUpload = !showDocUpload"
              >
                {{ showDocUpload ? 'Cancelar' : 'Subir documento' }}
              </AppButton>
            </div>

            <FormSection v-if="showDocUpload" title="Subir documento" :icon="IconUpload" class="mb-4">
              <form @submit.prevent="subirDocumento">
                <div class="row g-3">
                  <div class="col-md-4">
                    <label for="ad-archivo" class="form-label">Archivo</label>
                    <input id="ad-archivo" ref="docFileInput" type="file" class="form-control" accept=".pdf,.jpg,.jpeg,.png" required>
                    <small class="text-app-muted">PDF, JPG o PNG. Máximo 10MB.</small>
                  </div>
                  <div class="col-md-4">
                    <label for="ad-tipo" class="form-label">Tipo de documento</label>
                    <select id="ad-tipo" v-model="docFileType" class="form-select" required>
                      <option value="">Seleccionar…</option>
                      <option value="Resultados de laboratorio">Resultados de laboratorio</option>
                      <option value="Indicaciones médicas">Indicaciones médicas</option>
                      <option value="Historial externo">Historial externo</option>
                      <option value="Estudios en PDF">Estudios en PDF</option>
                      <option value="Documentos administrativos">Documentos administrativos</option>
                    </select>
                  </div>
                  <div class="col-md-4 d-flex align-items-end">
                    <AppButton type="submit" variant="success" block :icon="IconUpload" :loading="subiendoDoc">
                      Subir
                    </AppButton>
                  </div>
                </div>
              </form>
            </FormSection>

            <LoadingState v-if="cargandoDocs" label="Cargando documentos…" />
            <EmptyState
              v-else-if="documentos.length === 0"
              size="sm"
              :icon="IconDocument"
              title="Sin documentos asociados"
            />
            <div v-else class="row g-3">
              <div class="col-md-4 col-sm-6" v-for="d in documentos" :key="d.id">
                <BaseCard class="h-100 text-center">
                  <IconTile :icon="fileIconFor(d.fileName)" :tone="fileToneFor(d.fileName)" size="lg" class="mx-auto mb-2" />
                  <p class="doc__name">{{ d.fileName }}</p>
                  <small class="text-app-muted d-block mb-3">{{ formatDate(d.uploadedAt) }}</small>
                  <div class="d-flex justify-content-center gap-2">
                    <AppButton variant="soft" size="sm" :icon="IconView" @click="verDocumento(d)">Ver</AppButton>
                    <AppButton variant="soft-danger" size="sm" :icon="IconDelete" @click="confirmarEliminarDoc(d)">
                      Eliminar
                    </AppButton>
                  </div>
                </BaseCard>
              </div>
            </div>
          </div>

          <!-- IA TAB -->
          <div v-if="tabActual === 'ia'" class="animation-fade">

            <!-- Header con botón siempre visible -->
            <div class="d-flex justify-content-between align-items-center mb-4" v-if="!generandoIA">
              <div>
                <h6 class="fw-bold mb-0">Análisis de IA</h6>
                <small class="text-muted">{{ analisisIA.length }} análisis generado{{ analisisIA.length === 1 ? '' : 's' }}</small>
              </div>
              <button @click="solicitarAnalisisIA" class="btn btn-dark px-4" :disabled="solicitandoIA">
                <span v-if="solicitandoIA" class="spinner-border spinner-border-sm me-2"></span>
                {{ solicitandoIA ? 'Procesando...' : analisisIA.length === 0 ? 'Generar Reporte IA' : '+ Nuevo Análisis' }}
              </button>
            </div>

            <!-- Estado vacío descriptivo -->
            <EmptyState
              v-if="analisisIA.length === 0 && !generandoIA"
              :icon="IconAi"
              tone="brand"
              title="Asistente de Diagnóstico"
              message="La inteligencia artificial analizará el cuadro clínico, signos vitales y síntomas para sugerir recomendaciones y calcular factores de riesgo."
            />

            <div v-if="generandoIA" class="text-center py-5">
              <div class="spinner-border text-dark mb-3" style="width: 3rem; height: 3rem;" role="status"></div>
              <p class="text-muted">Generando análisis con inteligencia artificial...</p>
            </div>

            <div v-for="(ia, idx) in analisisIA" :key="ia.id || idx">
              <div class="bg-light p-4 rounded-4 border-start border-5 position-relative mb-4" :class="ia.isReviewed ? 'border-success' : 'border-info'">
                <span
                  class="badge position-absolute top-0 end-0 m-3 px-3 py-2 rounded-pill d-inline-flex align-items-center gap-1"
                  :class="ia.isReviewed ? 'bg-success' : 'bg-info'"
                >
                  <Icon :icon="ia.isReviewed ? IconCheck : IconSparkles" :size="13" />
                  {{ ia.isReviewed ? 'Revisado' : 'Nuevo' }}
                </span>

                <h5 class="fw-bold mb-3 d-flex align-items-center gap-2">
                  <Icon :icon="IconAi" :size="20" />
                  {{ ia.analysisType || 'Análisis Clínico' }}
                </h5>

                <template v-if="ia.aiResponse && !ia.aiResponse.startsWith('ERROR')">
                  <div class="mb-3" v-if="parsearRespuestaIA(ia.aiResponse)?.summary">
                    <h6 class="fw-bold text-dark mb-1">Resumen clínico:</h6>
                    <p class="text-muted">{{ parsearRespuestaIA(ia.aiResponse).summary }}</p>
                  </div>
                  <div class="mb-3" v-if="parsearRespuestaIA(ia.aiResponse)?.riskLevel">
                    <h6 class="fw-bold text-dark mb-1">Nivel de riesgo:</h6>
                    <span class="badge px-3 py-2 rounded-pill"
                      :class="{
                        'bg-success': parsearRespuestaIA(ia.aiResponse).riskLevel === 'Bajo',
                        'bg-warning text-dark': parsearRespuestaIA(ia.aiResponse).riskLevel === 'Medio',
                        'bg-danger': parsearRespuestaIA(ia.aiResponse).riskLevel === 'Alto'
                      }">
                      {{ parsearRespuestaIA(ia.aiResponse).riskLevel }}
                    </span>
                  </div>
                  <div class="mb-3" v-if="parsearRespuestaIA(ia.aiResponse)?.recommendations?.length">
                    <h6 class="fw-bold text-dark mb-1">Recomendaciones:</h6>
                    <ul class="list-unstyled mb-0">
                      <li v-for="(rec, i) in parsearRespuestaIA(ia.aiResponse).recommendations" :key="i" class="mb-2 ps-3 border-start border-2 border-info">
                        <strong class="text-dark">{{ rec.title }}</strong>
                        <p class="text-muted small mb-0">{{ rec.description }}</p>
                      </li>
                    </ul>
                  </div>
                  <div v-if="!parsearRespuestaIA(ia.aiResponse)" class="mb-3">
                    <h6 class="fw-bold text-dark mb-1">Resultado:</h6>
                    <pre class="bg-white border rounded-3 p-3 small text-muted"
                         style="white-space: pre-wrap; overflow-x: auto; max-height: 300px">{{ ia.aiResponse }}</pre>
                  </div>
                </template>
                <AppAlert v-else-if="ia.aiResponse?.startsWith('ERROR')" variant="danger" :message="ia.aiResponse" class="mb-3" />

                <div v-if="!ia.isReviewed" class="d-flex justify-content-end mt-3">
                  <button @click="marcarRevisado(ia)" class="btn btn-success btn-sm px-4 rounded-pill">Marcar como Revisado</button>
                </div>
              </div>
            </div>
          </div>

        </div>
      </div>
    </template>
    <ConfirmDialog
      :visible="deleteDocDialog"
      title="Eliminar Documento"
      :message="`¿Está seguro que desea eliminar el documento ${deleteDocTarget?.fileName}?`"
      confirmText="Eliminar"
      :danger="true"
      @confirm="eliminarDoc"
      @cancel="deleteDocDialog = false"
    />

    <DocumentPreviewModal v-model="showPreview" :doc="previewDoc" />
  </div>
</template>

<script setup>
import { ref, reactive, computed, markRaw, watch, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { appointmentService } from '@/api/appointments'
import { medicalRecordService } from '@/api/medicalRecords'
import { symptomService } from '@/api/symptoms'
import { vitalSignService } from '@/api/vitalSigns'
import { aiAnalysisService } from '@/api/aiAnalysis'
import { patientService } from '@/api/patients'
import { alertService } from '@/api/alerts'
import { recommendationService } from '@/api/recommendations'
import { medicalDocumentService } from '@/api/medicalDocuments'
import {
  AppAlert,
  AppButton,
  BaseCard,
  ConfirmDialog,
  DataField,
  DocumentPreviewModal,
  EmptyState,
  FormSection,
  Icon,
  IconTile,
  LoadingState,
  PageHeader,
  StatusBadge,
  TabNav
} from '@/components/ui'
import {
  IconAi,
  IconAlert,
  IconAppointment,
  IconCheck,
  IconClose,
  IconDelete,
  IconDocument,
  IconEdit,
  IconNotes,
  IconRecommendation,
  IconRecord,
  IconSparkles,
  IconSymptoms,
  IconUpload,
  IconUser,
  IconView,
  IconVitals
} from '@/lib/icons'
import { fileIconFor, fileToneFor, isPdfFile } from '@/lib/fileIcons'
import { statusVariant, translateStatus } from '@/utils/appointmentStatus'
import { authStore } from '@/stores/auth'

const props = defineProps({
  id: { type: String, required: true }
})

const router = useRouter()
const auth = authStore

const loading = ref(true)
const error = ref('')
const cita = ref(null)
const patientName = ref('')
const tabActual = ref('info')
const nuevoEstado = ref('Pending')

// `markRaw` como en config/navigation.js: los iconos son componentes, no datos,
// y envolverlos en un proxy reactivo dispara un aviso de Vue.
const TABS = [
  { id: 'info', label: 'Info Básica', icon: markRaw(IconNotes) },
  { id: 'sintomas', label: 'Síntomas', icon: markRaw(IconSymptoms) },
  { id: 'signos', label: 'Signos Vitales', icon: markRaw(IconVitals) },
  { id: 'ia', label: 'Análisis IA', icon: markRaw(IconAi), hideForNurse: true },
  { id: 'records', label: 'Historial Médico', icon: markRaw(IconRecord), hideForNurse: true },
  { id: 'alertas', label: 'Alertas', icon: markRaw(IconAlert) },
  { id: 'recomendaciones', label: 'Recomendaciones', icon: markRaw(IconRecommendation), hideForNurse: true },
  { id: 'documentos', label: 'Documentos', icon: markRaw(IconDocument) }
]

const tabsVisibles = computed(() =>
  TABS.filter((t) => !(t.hideForNurse && auth.hasRole('Nurse')))
)

const guardandoNotas = ref(false)
const notasGuardadas = ref(false)
const notasEdit = ref('')

const sinstomasCargados = ref(false)
const showSintomaForm = ref(false)
const guardandoSintoma = ref(false)
const sintomas = ref([])
const sintomaForm = reactive({
  name: '', severity: 'Moderado', startedAt: '', notes: ''
})

const showSignosForm = ref(false)
const guardandoSignos = ref(false)
const signos = ref([])
const signosForm = reactive({
  temperature: 36.5, heartRate: 72, systolicPressure: 120,
  diastolicPressure: 80, oxygenSaturation: 98, glucose: null
})

const analisisIA = ref([])
const solicitandoIA = ref(false)
const generandoIA = ref(false)

const editSintomaId = ref(null)
const editSintomaForm = reactive({ name: '', severity: 'Moderado', startedAt: '', notes: '' })
const deleteSintomaDialog = ref(false)
const deleteSintomaTarget = ref(null)
const deleteSintomaLoading = ref(false)

const editSignoId = ref(null)
const editSignoForm = reactive({ temperature: 36.5, heartRate: 72, systolicPressure: 120, diastolicPressure: 80, oxygenSaturation: 98, glucose: null })

const recordsMedicos = ref([])
const showRecordForm = ref(false)
const editRecordId = ref(null)
const guardandoRecord = ref(false)
const recordForm = reactive({ diagnosisInitial: '', notes: '', antecedentes: '', observacionesConsulta: '' })

const alertas = ref([])
const cargandoAlertas = ref(false)

const recomendaciones = ref([])
const cargandoRecomendaciones = ref(false)

const documentos = ref([])
const cargandoDocs = ref(false)
const showDocUpload = ref(false)
const subiendoDoc = ref(false)
const docFileInput = ref(null)
const docFileType = ref('')

const deleteDocDialog = ref(false)
const deleteDocTarget = ref(null)

const showPreview = ref(false)
const previewDoc = ref(null)

const ultimoSigno = computed(() => {
  if (!signos.value.length) return { temperature: 0, heartRate: 0, systolicPressure: 0, diastolicPressure: 0, oxygenSaturation: 0, glucose: null }
  return signos.value[signos.value.length - 1]
})

const formatDateTime = (dateStr) => {
  if (!dateStr) return '-'
  try {
    return new Date(dateStr).toLocaleDateString('es-ES', {
      day: '2-digit', month: 'short', year: 'numeric', hour: '2-digit', minute: '2-digit'
    })
  } catch { return dateStr }
}

const formatDate = (dateStr) => {
  if (!dateStr) return '-'
  try {
    return new Date(dateStr).toLocaleDateString('es-ES', { day: '2-digit', month: 'short', year: 'numeric' })
  } catch { return dateStr }
}

// Mismo mapa que NurseFollowUpView: las severidades del dominio se traducen a
// variantes de StatusBadge, no a clases `bg-*` de Bootstrap.
const SEVERITY_VARIANTS = { Leve: 'low', Moderado: 'moderate', Severo: 'severe' }
const severityVariant = (sev) => SEVERITY_VARIANTS[sev] ?? 'secondary'

const cargarDetalle = async () => {
  loading.value = true
  error.value = ''
  try {
    const res = await appointmentService.getDetail(props.id)
    cita.value = res.data
    nuevoEstado.value = res.data.status || 'Pending'
    notasEdit.value = res.data.notes || ''

    if (res.data.patientId) {
      try {
        const pRes = await patientService.getById(res.data.patientId)
        patientName.value = pRes.data?.fullName || `Paciente #${res.data.patientId}`
      } catch {
        patientName.value = `Paciente #${res.data.patientId}`
      }
    }

    sintomas.value = res.data.symptoms || []
    sinstomasCargados.value = true
    signos.value = res.data.vitalSigns || []
    analisisIA.value = res.data.aiAnalyses || []
  } catch (err) {
    if (err.response?.status === 404) {
      error.value = 'Cita no encontrada.'
    } else {
      error.value = 'Error al cargar detalle de la cita.'
    }
  } finally {
    loading.value = false
  }
}

const guardarNotas = async () => {
  guardandoNotas.value = true
  notasGuardadas.value = false
  try {
    await appointmentService.update(Number(props.id), {
      patientId: cita.value.patientId,
      doctorId: cita.value.doctorId,
      appointmentDate: cita.value.appointmentDate,
      status: cita.value.status,
      reason: cita.value.reason,
      notes: notasEdit.value
    })
    notasGuardadas.value = true
    cita.value.notes = notasEdit.value
    setTimeout(() => { notasGuardadas.value = false }, 3000)
  } catch (err) {
    error.value = 'Error al guardar las notas'
  } finally {
    guardandoNotas.value = false
  }
}

const cambiarEstado = async () => {
  try {
    await appointmentService.changeStatus(Number(props.id), nuevoEstado.value)
    cita.value.status = nuevoEstado.value
  } catch (err) {
    nuevoEstado.value = cita.value.status
    error.value = 'Error al cambiar estado'
  }
}

const toggleFormSintoma = () => {
  showSintomaForm.value = !showSintomaForm.value
  if (!showSintomaForm.value) {
    sintomaForm.name = ''
    sintomaForm.severity = 'Moderado'
    sintomaForm.startedAt = ''
    sintomaForm.notes = ''
  }
}

const sintomaError = ref('')

const guardarSintoma = async () => {
  if (!sintomaForm.name || !sintomaForm.startedAt) return
  guardandoSintoma.value = true
  sintomaError.value = ''
  try {
    const res = await symptomService.create({
      appointmentId: Number(props.id),
      name: sintomaForm.name,
      severity: sintomaForm.severity,
      startedAt: sintomaForm.startedAt,
      notes: sintomaForm.notes || null
    })
    sintomas.value.push(res.data)
    sintomaForm.name = ''
    sintomaForm.startedAt = ''
    sintomaForm.notes = ''
    showSintomaForm.value = false
  } catch (err) {
    const data = err.response?.data
    sintomaError.value = data?.detail || data?.message || 'Error al guardar síntoma'
  } finally {
    guardandoSintoma.value = false
  }
}

const toggleFormSignos = () => {
  showSignosForm.value = !showSignosForm.value
  if (!showSignosForm.value) {
    signosForm.temperature = 36.5
    signosForm.heartRate = 72
    signosForm.systolicPressure = 120
    signosForm.diastolicPressure = 80
    signosForm.oxygenSaturation = 98
    signosForm.glucose = null
  }
}

const signosError = ref('')
const recordError = ref('')

const guardarSignos = async () => {
  guardandoSignos.value = true
  signosError.value = ''
  try {
    const res = await vitalSignService.create({
      appointmentId: Number(props.id),
      temperature: signosForm.temperature,
      heartRate: signosForm.heartRate,
      systolicPressure: signosForm.systolicPressure,
      diastolicPressure: signosForm.diastolicPressure,
      oxygenSaturation: signosForm.oxygenSaturation,
      glucose: signosForm.glucose || null
    })
    signos.value.push(res.data)
    showSignosForm.value = false
  } catch (err) {
    const data = err.response?.data
    signosError.value = data?.detail || data?.message || 'Error al guardar signos vitales'
  } finally {
    guardandoSignos.value = false
  }
}

const solicitarAnalisisIA = async () => {
  solicitandoIA.value = true
  generandoIA.value = true
  try {
    const res = await aiAnalysisService.generate({
      appointmentId: Number(props.id),
      analysisType: 'ClinicalAnalysis',
      request: 'Analiza los signos vitales, síntomas e historial clínico del paciente'
    })
    analisisIA.value.unshift(res.data)
    setTimeout(async () => {
      try {
        const detailRes = await appointmentService.getDetail(props.id)
        analisisIA.value = detailRes.data.aiAnalyses || []
      } catch {
        // fallback: mantener lo que ya tenemos
      }
      generandoIA.value = false
      solicitandoIA.value = false
    }, 2000)
  } catch (err) {
    const data = err.response?.data
    error.value = data?.detail || data?.title || 'Error al generar análisis IA'
    generandoIA.value = false
    solicitandoIA.value = false
  }
}

const parsearRespuestaIA = (raw) => {
  if (!raw) return null
  try {
    const cleaned = raw.trim()
      .replace(/^```json\s*/i, '')
      .replace(/^```\s*/, '')
      .replace(/\s*```$/, '')
    return JSON.parse(cleaned)
  } catch { return null }
}

const marcarRevisado = async (ia) => {
  try {
    await aiAnalysisService.markAsReviewed(ia.id)
    ia.isReviewed = true
  } catch (err) {
    error.value = 'Error al marcar como revisado'
  }
}

const editarSintoma = (s) => {
  editSintomaId.value = s.id
  editSintomaForm.name = s.name
  editSintomaForm.severity = s.severity
  editSintomaForm.startedAt = s.startedAt ? s.startedAt.substring(0, 10) : ''
  editSintomaForm.notes = s.notes || ''
}

const cancelarEditSintoma = () => {
  editSintomaId.value = null
}

const actualizarSintoma = async () => {
  guardandoSintoma.value = true
  try {
    const res = await symptomService.update(editSintomaId.value, { appointmentId: Number(props.id), ...editSintomaForm })
    const idx = sintomas.value.findIndex(s => s.id === editSintomaId.value)
    if (idx !== -1) sintomas.value[idx] = res.data
    editSintomaId.value = null
  } catch (err) {
    const data = err.response?.data
    sintomaError.value = data?.detail || data?.message || 'Error al actualizar síntoma'
  } finally {
    guardandoSintoma.value = false
  }
}

const confirmarEliminarSintoma = (s) => {
  deleteSintomaTarget.value = s
  deleteSintomaDialog.value = true
}

const eliminarSintoma = async () => {
  deleteSintomaLoading.value = true
  try {
    await symptomService.remove(deleteSintomaTarget.value.id)
    sintomas.value = sintomas.value.filter(s => s.id !== deleteSintomaTarget.value.id)
  } catch (err) {
    error.value = 'Error al eliminar síntoma'
  } finally {
    deleteSintomaDialog.value = false
    deleteSintomaLoading.value = false
  }
}

const editarSigno = (s) => {
  editSignoId.value = s.id
  editSignoForm.temperature = s.temperature
  editSignoForm.heartRate = s.heartRate
  editSignoForm.systolicPressure = s.systolicPressure
  editSignoForm.diastolicPressure = s.diastolicPressure
  editSignoForm.oxygenSaturation = s.oxygenSaturation
  editSignoForm.glucose = s.glucose
}

const cancelarEditSigno = () => {
  editSignoId.value = null
}

const actualizarSigno = async () => {
  guardandoSignos.value = true
  try {
    const res = await vitalSignService.update(editSignoId.value, { ...editSignoForm })
    const idx = signos.value.findIndex(s => s.id === editSignoId.value)
    if (idx !== -1) signos.value[idx] = res.data
    editSignoId.value = null
  } catch (err) {
    error.value = 'Error al actualizar signos vitales'
  } finally {
    guardandoSignos.value = false
  }
}

const toggleFormRecord = () => {
  showRecordForm.value = !showRecordForm.value
  if (!showRecordForm.value) cancelarFormRecord()
}

const cancelarFormRecord = () => {
  showRecordForm.value = false
  editRecordId.value = null
  recordForm.diagnosisInitial = ''
  recordForm.notes = ''
  recordForm.antecedentes = ''
  recordForm.observacionesConsulta = ''
}

const guardarRecord = async () => {
  guardandoRecord.value = true
  try {
    const payload = {
      appointmentId: Number(props.id),
      patientId: cita.value.patientId,
      diagnosisInitial: recordForm.diagnosisInitial || null,
      notes: recordForm.notes || null,
      antecedentes: recordForm.antecedentes || null,
      observacionesConsulta: recordForm.observacionesConsulta || null
    }
    if (editRecordId.value) {
      const res = await medicalRecordService.update(editRecordId.value, payload)
      const idx = recordsMedicos.value.findIndex(r => r.id === editRecordId.value)
      if (idx !== -1) recordsMedicos.value[idx] = res.data
    } else {
      const res = await medicalRecordService.create(payload)
      recordsMedicos.value.push(res.data)
    }
    cancelarFormRecord()
  } catch (err) {
    const data = err.response?.data
    recordError.value = data?.detail || data?.message || 'Error al guardar registro médico'
  } finally {
    guardandoRecord.value = false
  }
}

const editarRecord = (r) => {
  editRecordId.value = r.id
  recordForm.diagnosisInitial = r.diagnosisInitial || ''
  recordForm.notes = r.notes || ''
  recordForm.antecedentes = r.antecedentes || ''
  recordForm.observacionesConsulta = r.observacionesConsulta || ''
  showRecordForm.value = true
}

const confirmarEliminarDoc = (d) => {
  deleteDocTarget.value = d
  deleteDocDialog.value = true
}

// Los PDF no se previsualizan dentro del modal: el visor nativo del navegador
// en una pestaña aparte es mejor que un <embed> encajado (mismo criterio que
// MedicalDocumentsView).
const verDocumento = (d) => {
  if (isPdfFile(d.fileName)) {
    window.open(`/api/v1/MedicalDocument/${d.id}/file`, '_blank')
    return
  }
  previewDoc.value = d
  showPreview.value = true
}

const subirDocumento = async () => {
  const file = docFileInput.value?.files?.[0]
  if (!file) return
  subiendoDoc.value = true
  try {
    const formData = new FormData()
    formData.append('file', file)
    formData.append('fileName', file.name)
    formData.append('fileType', docFileType.value)
    formData.append('appointmentId', props.id)
    formData.append('patientId', cita.value.patientId)
    await medicalDocumentService.upload(formData)
    showDocUpload.value = false
    docFileType.value = ''
    docFileInput.value.value = ''
    await cargarDocumentos()
  } catch (err) {
    error.value = 'Error al subir el documento'
  } finally {
    subiendoDoc.value = false
  }
}

const cargarAlertas = async () => {
  cargandoAlertas.value = true
  try {
    const res = await alertService.getByAppointment(props.id)
    alertas.value = res.data || []
  } catch {
    alertas.value = []
  } finally {
    cargandoAlertas.value = false
  }
}

const cargarRecomendaciones = async () => {
  cargandoRecomendaciones.value = true
  try {
    const res = await recommendationService.getByAppointment(props.id)
    recomendaciones.value = res.data || []
  } catch {
    recomendaciones.value = []
  } finally {
    cargandoRecomendaciones.value = false
  }
}

const cargarDocumentos = async () => {
  cargandoDocs.value = true
  try {
    const res = await medicalDocumentService.getByAppointment(props.id)
    documentos.value = res.data || []
  } catch {
    documentos.value = []
  } finally {
    cargandoDocs.value = false
  }
}

const eliminarDoc = async () => {
  try {
    await medicalDocumentService.remove(deleteDocTarget.value.id)
    documentos.value = documentos.value.filter(d => d.id !== deleteDocTarget.value.id)
  } catch (err) {
    error.value = 'Error al eliminar el documento'
  } finally {
    deleteDocDialog.value = false
  }
}

watch(tabActual, (tab) => {
  if (tab === 'alertas' && alertas.value.length === 0) cargarAlertas()
  if (tab === 'recomendaciones' && recomendaciones.value.length === 0) cargarRecomendaciones()
  if (tab === 'documentos' && documentos.value.length === 0) cargarDocumentos()
  if (tab === 'records' && recordsMedicos.value.length === 0 && cita.value?.medicalRecords) {
    recordsMedicos.value = cita.value.medicalRecords || []
  }
})

onMounted(() => {
  cargarDetalle()
})
</script>

<style scoped>
/* `.animation-fade` se mantiene como alias local de la utilidad global u-fade-in
   para no tocar los ocho `class="animation-fade"` de las pestañas. */
.animation-fade {
  animation: u-fade-in 0.24s ease-out;
}

.sintoma {
  height: 100%;
  padding: 0.875rem 1rem;
  background-color: var(--app-surface-sunken);
  border: 1px solid var(--app-border);
  border-inline-start: 3px solid var(--app-border-strong);
  border-radius: var(--app-radius-lg);
}

.sintoma--low {
  border-inline-start-color: var(--c-success-500);
}
.sintoma--moderate {
  border-inline-start-color: var(--c-warning-500);
}
.sintoma--severe {
  border-inline-start-color: var(--c-danger-500);
}

.sintoma__nombre {
  margin: 0;
  font-size: 0.9375rem;
  font-weight: 600;
  color: var(--app-text-strong);
}

.sintoma__notas {
  margin: 0.5rem 0 0;
  font-size: 0.85rem;
  color: var(--app-text-muted);
}

.doc__name {
  margin: 0 0 0.25rem;
  font-weight: 600;
  font-size: 0.875rem;
  color: var(--app-text-strong);
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}
</style>
