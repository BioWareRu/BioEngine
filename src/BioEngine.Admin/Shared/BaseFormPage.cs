using System;
using System.Threading.Tasks;
using AntDesign;
using BioEngine.Core.Data.Entities.Abstractions;
using BioEngine.Core.Data.Repositories;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.DependencyInjection;
using Sitko.Core.Repository;

namespace BioEngine.Admin.Shared
{
    public abstract class BaseFormPage : BasePage
    {
        public Task NotifyStateChangeAsync()
        {
            return InvokeAsync(StateHasChanged);
        }
    }

    public abstract class BaseFormPage<TEntity, TEntityPk, TFormModel> : BaseFormPage
        where TEntity : class, IEntity<TEntityPk>, new()
        where TFormModel : BaseFormModel<TEntity, TEntityPk>
    {
        protected IRepository<TEntity, TEntityPk> Repository;
        protected Form<TFormModel>? Form { get; set; }
        protected TFormModel FormModel { get; set; }
        [Inject] protected NavigationManager NavigationManager { get; set; }
        [Inject] protected NotificationService NotificationService { get; set; }
        protected bool IsNew { get; private set; } = true;

        private TEntityPk _entityPk;

        [Parameter]
        public TEntityPk? EntityId
        {
            get => _entityPk;
            set
            {
                if (value is not null && !value.Equals(default))
                {
                    _entityPk = value;
                    IsNew = false;
                }
            }
        }


        protected override async Task OnInitializedAsync()
        {
            Repository = ScopedServices.GetRequiredService<IRepository<TEntity, TEntityPk>>();
            TEntity? entity;
            if (!IsNew)
            {
                entity = await Repository.GetByIdAsync(EntityId);
            }
            else
            {
                entity = await Repository.NewAsync();
            }

            if (entity == null)
            {
                throw new Exception("Can't find entity");
            }

            FormModel = await CreateFormModelAsync(entity);
            await base.OnInitializedAsync();
        }

        protected abstract Task<TFormModel> CreateFormModelAsync(TEntity entity);

        protected async Task OnFinishFailedAsync(EditContext editContext)
        {
            await NotificationService.Error(new NotificationConfig
            {
                Message = "Ошибка", Description = string.Join(". ", editContext.GetValidationMessages())
            });
        }

        protected async Task OnFinishAsync(EditContext editContext)
        {
            StartLoading();
            var entity = FormModel.GetEntity();
            try
            {
                var result = IsNew
                    ? await Repository.AddAsync(entity)
                    : await Repository.UpdateAsync(entity);
                StopLoading();
                if (result.IsSuccess)
                {
                    if (IsNew)
                    {
                        await OnCreatedAsync(entity);
                        StateHasChanged();
                    }
                    else
                    {
                        await NotificationService.Success(new NotificationConfig
                        {
                            Message = "Успех",
                            Description = "Запись успешно сохранёна",
                            Placement = NotificationPlacement.BottomRight
                        });
                    }
                }
                else
                {
                    await NotificationService.Error(new NotificationConfig
                    {
                        Message = "Ошибка",
                        Description = result.ErrorsString,
                        Placement = NotificationPlacement.BottomRight
                    });
                }
            }
            catch (Exception ex)
            {
                StopLoading();
                await NotificationService.Error(new NotificationConfig
                {
                    Message = "Критическая ошибка",
                    Description = ex.ToString(),
                    Placement = NotificationPlacement.BottomRight
                });
            }
        }

        protected virtual Task OnCreatedAsync(TEntity entity)
        {
            return Task.CompletedTask;
        }

        protected void Save()
        {
            Form?.Submit();
        }

        protected bool CanSave()
        {
            if (Form == null)
            {
                return false;
            }

            if (IsNew)
            {
                return Form.Validate();
            }

            var changes = Repository.GetChanges(FormModel.GetEntity(), new TEntity());
            return changes.Length > 0 && Form.Validate();
        }
    }

    public abstract class
        BasePublishableFormPage<TEntity, TEntityPk, TFormModel> : BaseFormPage<TEntity, TEntityPk, TFormModel>
        where TEntity : class, IEntity<TEntityPk>, IPublishable, new()
        where TFormModel : BaseFormModel<TEntity, TEntityPk>
    {
        protected bool CanPublish()
        {
            if (CanSave())
            {
                return false;
            }

            if (IsNew)
            {
                return false;
            }

            return !FormModel.GetEntity().IsPublished;
        }

        protected bool CanUnPublish()
        {
            if (CanSave())
            {
                return false;
            }

            if (IsNew)
            {
                return false;
            }

            return FormModel.GetEntity().IsPublished;
        }

        protected async Task Publish()
        {
            if (CanPublish() && Repository is IPublishableEntityRepository<TEntity> publishableEntityRepository)
            {
                StartLoading();
                var entity = FormModel.GetEntity();
                try
                {
                    await publishableEntityRepository.PublishAsync(entity);
                    StopLoading();
                    await NotificationService.Success(new NotificationConfig
                    {
                        Message = "Успех",
                        Description = "Запись успешно опубликована",
                        Placement = NotificationPlacement.BottomRight
                    });
                }
                catch (Exception ex)
                {
                    StopLoading();
                    await NotificationService.Error(new NotificationConfig
                    {
                        Message = "Критическая ошибка",
                        Description = ex.ToString(),
                        Placement = NotificationPlacement.BottomRight
                    });
                }
            }
        }

        protected async Task UnPublish()
        {
            if (CanUnPublish() && Repository is IPublishableEntityRepository<TEntity> publishableEntityRepository)
            {
                StartLoading();
                var entity = FormModel.GetEntity();
                try
                {
                    await publishableEntityRepository.UnPublishAsync(entity);
                    StopLoading();
                    await NotificationService.Success(new NotificationConfig
                    {
                        Message = "Успех",
                        Description = "Запись успешно спрятана",
                        Placement = NotificationPlacement.BottomRight
                    });
                }
                catch (Exception ex)
                {
                    StopLoading();
                    await NotificationService.Error(new NotificationConfig
                    {
                        Message = "Критическая ошибка",
                        Description = ex.ToString(),
                        Placement = NotificationPlacement.BottomRight
                    });
                }
            }
        }
    }

    public abstract class BaseFormModel<TEntity, TEntityPk> where TEntity : class, IEntity<TEntityPk>
    {
        public abstract TEntity GetEntity();
    }

    public class ChangesDetector : ComponentBase
    {
        [CascadingParameter] public EditContext CurrentEditContext { get; set; }

        [Parameter] public BaseFormPage FormPage { get; set; }

        protected override void OnInitialized()
        {
            if (CurrentEditContext == null)
            {
                throw new InvalidOperationException($"{nameof(ChangesDetector)} requires a cascading " +
                                                    $"parameter of type {nameof(EditContext)}. For example, you can use {nameof(DataAnnotationsValidator)} " +
                                                    $"inside an EditForm.");
            }

            CurrentEditContext.OnFieldChanged += async (sender, args) =>
            {
                await FormPage.NotifyStateChangeAsync();
            };
        }
    }
}
