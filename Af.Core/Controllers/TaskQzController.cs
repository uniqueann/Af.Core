using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Af.Core.Common;
using Af.Core.Common.Converter;
using Af.Core.IRepository.UnitOfWork;
using Af.Core.IServices;
using Af.Core.Model.Models;
using Af.Core.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Af.Core.Controllers
{
    /// <summary>
    /// 计划任务
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(Permissions.Name)]
    public class TaskQzController : ControllerBase
    {
        private readonly ITaskQzServices _taskQzServices;
        private readonly ISchedulerCenter _schedulerCenter;
        private readonly IUnitOfWork _unitOfWork;

        public TaskQzController(ITaskQzServices taskQzServices, ISchedulerCenter schedulerCenter, IUnitOfWork unitOfWork)
        {
            _taskQzServices = taskQzServices;
            _schedulerCenter = schedulerCenter;
            _unitOfWork = unitOfWork;
        }



        /// <summary>
        /// 分页获取计划任务
        /// </summary>
        /// <param name="page"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<MessageModel<PageModel<TaskQz>>> Get(int page = 1, string key = "")
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrWhiteSpace(key)) key = "";
            int pageSize = 50;

            var data = await _taskQzServices.QueryPage(a=>a.IsDeleted==false && (a.Name!=null && a.Name.Contains(key)),page,pageSize," Id Desc");
            if (data.Total > 0)
            {
                foreach (var item in data.PageData)
                {
                    item.Triggers = await _schedulerCenter.GetTaskStaus(item);
                }
            }

            return MessageModel<PageModel<TaskQz>>.Message(true, "获取成功", data);
        }

        /// <summary>
        /// 新增计划任务
        /// </summary>
        /// <param name="taskQz"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<MessageModel<string>> Post(TaskQz taskQz)
        {
            var data = new MessageModel<string>();
            _unitOfWork.BeginTran();
            var id = (await _taskQzServices.Add(taskQz));
            data.success = id > 0;
            try
            {
                if (data.success)
                {
                    taskQz.Id = id;
                    data.response = id.ObjToString();
                    data.msg = "添加成功";
                    var resuModel = await _schedulerCenter.AddScheduleJobAsync(taskQz);
                    data.success = resuModel.success;
                    if (resuModel.success)
                    {
                        data.msg = $"{data.msg}=>启动成功=>{resuModel.msg}";
                    }
                    else
                    {
                        data.msg = $"{data.msg}=>启动失败=>{resuModel.msg}";
                    }
                }
                else
                {
                    data.msg = "添加失败";
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (data.success) _unitOfWork.CommitTran();
                else _unitOfWork.RollbackTran();
            }
            return data;
        }

        /// <summary>
        /// 修改计划任务
        /// </summary>
        /// <param name="taskQz"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<MessageModel<string>> Put(TaskQz taskQz)
        {
            var data = new MessageModel<string>();

            if (taskQz!=null && taskQz.Id>0)
            {
                _unitOfWork.BeginTran();
                data.success = await _taskQzServices.Update(taskQz);
                try
                {
                    if (data.success)
                    {
                        data.msg = "修改成功";
                        data.response = taskQz.Id.ObjToString();
                        if (taskQz.IsStart)
                        {
                            var resuModelStop = await _schedulerCenter.StopScheduleJobAsync(taskQz);
                            data.msg = $"{data.msg}=>停止: {resuModelStop.msg}";
                            var resuModelStart = await _schedulerCenter.AddScheduleJobAsync(taskQz);
                            data.success = resuModelStart.success;
                            data.msg = $"{data.msg}=>启动: {resuModelStart.msg}";
                        }
                        else
                        {
                            var resuModelStop = await _schedulerCenter.StopScheduleJobAsync(taskQz);
                            data.msg = $"{data.msg}=>停止: {resuModelStop.msg}";
                        }
                    }
                    else
                    {
                        data.msg = "修改失败";
                    }
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    if (data.success)
                        _unitOfWork.CommitTran();
                    else
                        _unitOfWork.RollbackTran();
                }
            }
            return data;
        }

        /// <summary>
        /// 删除一个任务
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<MessageModel<string>> Delete(int jobId)
        {
            var data = new MessageModel<string>();
            var model = await _taskQzServices.QueryById(jobId);
            if (model != null)
            {
                _unitOfWork.BeginTran();
                data.success = await _taskQzServices.DeleteById(jobId);
                try
                {
                    data.response = model.Id.ToString();
                    if (data.success)
                    {
                        data.msg = "删除成功";
                        var resuModel = await _schedulerCenter.StopScheduleJobAsync(model);
                        data.msg = $"{data.msg}=>任务状态=>{resuModel.msg}";
                    }
                    else
                    {
                        data.msg = "删除失败";
                    }
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    if (data.success)
                        _unitOfWork.CommitTran();
                    else
                        _unitOfWork.RollbackTran();
                }
            }
            else
            {
                data.msg = "任务不存在";
            }
            return data;
        }

        /// <summary>
        /// 启动计划任务
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<MessageModel<string>> StartJob(int jobId)
        {
            var data = new MessageModel<string>();
            var model = await _taskQzServices.QueryById(jobId);
            if (model != null)
            {
                _unitOfWork.BeginTran();
                try
                {
                    model.IsStart = true;
                    data.success = await _taskQzServices.Update(model);
                    data.response = model.Id.ObjToString();
                    if (data.success)
                    {
                        data.msg = "更新成功";
                        var resuModel = await _schedulerCenter.AddScheduleJobAsync(model);
                        data.success = resuModel.success;
                        if (resuModel.success)
                        {
                            data.msg = $"{data.msg}=>启动成功=>{resuModel.msg}";
                        }
                        else
                        {
                            data.msg = $"{data.msg}=>启动失败=>{resuModel.msg}";
                        }
                    }
                    else
                    {
                        data.msg = "更新失败";
                    }
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    if (data.success)
                        _unitOfWork.CommitTran();
                    else
                        _unitOfWork.RollbackTran();
                }
            }
            else
            {
                data.msg = "任务不存在";
            }
            return data;
        }
        /// <summary>
        /// 停止计划任务
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<MessageModel<string>> StopJob(int jobId)
        {
            var data = new MessageModel<string>();
            var model = await _taskQzServices.QueryById(jobId);
            if (model != null)
            {
                _unitOfWork.BeginTran();
                try
                {
                    model.IsStart = true;
                    data.success = await _taskQzServices.Update(model);
                    data.response = model.Id.ObjToString();
                    if (data.success)
                    {
                        data.msg = "更新成功";
                        var resuModel = await _schedulerCenter.StopScheduleJobAsync(model);
                        data.success = resuModel.success;
                        if (resuModel.success)
                        {
                            data.msg = $"{data.msg}=>停止成功=>{resuModel.msg}";
                        }
                        else
                        {
                            data.msg = $"{data.msg}=>停止失败=>{resuModel.msg}";
                        }
                    }
                    else
                    {
                        data.msg = "更新失败";
                    }
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    if (data.success)
                        _unitOfWork.CommitTran();
                    else
                        _unitOfWork.RollbackTran();
                }
            }
            else
            {
                data.msg = "任务不存在";
            }
            return data;
        }

        /// <summary>
        /// 暂停任务计划
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<MessageModel<string>> PauseJob(int jobId)
        {
            var data = new MessageModel<string>();
            var model = await _taskQzServices.QueryById(jobId);
            if (model != null)
            {
                _unitOfWork.BeginTran();
                try
                {
                    model.IsStart = true;
                    data.success = await _taskQzServices.Update(model);
                    data.response = model.Id.ObjToString();
                    if (data.success)
                    {
                        data.msg = "更新成功";
                        var resuModel = await _schedulerCenter.PauseJob(model);
                        data.success = resuModel.success;
                        if (resuModel.success)
                        {
                            data.msg = $"{data.msg}=>暂停成功=>{resuModel.msg}";
                        }
                        else
                        {
                            data.msg = $"{data.msg}=>暂停失败=>{resuModel.msg}";
                        }
                    }
                    else
                    {
                        data.msg = "更新失败";
                    }
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    if (data.success)
                        _unitOfWork.CommitTran();
                    else
                        _unitOfWork.RollbackTran();
                }
            }
            else
            {
                data.msg = "任务不存在";
            }
            return data;
        }

        /// <summary>
        /// 恢复任务计划
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<MessageModel<string>> ResumeJob(int jobId)
        {
            var data = new MessageModel<string>();
            var model = await _taskQzServices.QueryById(jobId);
            if (model != null)
            {
                _unitOfWork.BeginTran();
                try
                {
                    model.IsStart = true;
                    data.success = await _taskQzServices.Update(model);
                    data.response = model.Id.ObjToString();
                    if (data.success)
                    {
                        data.msg = "更新成功";
                        var resuModel = await _schedulerCenter.ResumeJob(model);
                        data.success = resuModel.success;
                        if (resuModel.success)
                        {
                            data.msg = $"{data.msg}=>恢复成功=>{resuModel.msg}";
                        }
                        else
                        {
                            data.msg = $"{data.msg}=>恢复失败=>{resuModel.msg}";
                        }
                    }
                    else
                    {
                        data.msg = "更新失败";
                    }
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    if (data.success)
                        _unitOfWork.CommitTran();
                    else
                        _unitOfWork.RollbackTran();
                }
            }
            else
            {
                data.msg = "任务不存在";
            }
            return data;
        }


        /// <summary>
        /// 重启任务计划
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<MessageModel<string>> ReCovery(int jobId)
        {
            var data = new MessageModel<string>();
            var model = await _taskQzServices.QueryById(jobId);
            if (model != null)
            {
                _unitOfWork.BeginTran();
                try
                {
                    model.IsStart = true;
                    data.success = await _taskQzServices.Update(model);
                    data.response = model.Id.ObjToString();
                    if (data.success)
                    {
                        data.msg = "更新成功";
                        var resuModelStop = await _schedulerCenter.StopScheduleJobAsync(model);
                        var resuModel = await _schedulerCenter.AddScheduleJobAsync(model);
                        data.success = resuModel.success;
                        data.msg = $"{data.msg}=>停止: {resuModelStop.msg}=>启动: {resuModel.msg}";
                    }
                    else
                    {
                        data.msg = "更新失败";
                    }
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    if (data.success)
                        _unitOfWork.CommitTran();
                    else
                        _unitOfWork.RollbackTran();
                }
            }
            else
            {
                data.msg = "任务不存在";
            }
            return data;
        }

    }
}
