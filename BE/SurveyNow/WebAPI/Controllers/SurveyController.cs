﻿using Application.DTOs.Request.Survey;
using Application.DTOs.Response;
using Application.DTOs.Response.Survey;
using Application.ErrorHandlers;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SurveyNow.Controllers;

[Route("api/v1/surveys")]
[ApiController]
[Authorize]
[Produces("application/json")]
public class SurveyController : ControllerBase
{
    private readonly ISurveyService _surveyService;
    private readonly ILogger<SurveyController> _logger;

    public SurveyController(ISurveyService surveyService, ILogger<SurveyController> logger)
    {
        _surveyService = surveyService;
        _logger = logger;
    }

    /// <summary>
    /// User can create a survey. If user buy pack before create survey, we need to call create survey API
    /// before call API to add pack to survey. This make sure survey is created before add pack
    /// and make sure pack is valid bought
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(SurveyDetailResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorDetail))]
    public async Task<ActionResult<SurveyDetailResponse>> CreateSurveyAsync([FromBody] SurveyRequest request)
    {
        var result = await _surveyService.CreateSurveyAsync(request);
        return Created(nameof(CommonFilterAsync), result);
    }

    /// <summary>
    /// Người dùng trả lời khảo sát bằng cách cung cấp một danh sách câu trả lời.
    /// Câu hỏi bao gồm các rowOption và Column option (trừ câu hỏi dạng other hoặc dạng rating).
    /// Người tạo khảo sát ko được trả lời khảo sát của mình và mỗi người chỉ đươc trả lời khảo sát 1 lần
    /// </summary>
    /// <param name="request"></param>
    ///<returns></returns>
    ///     [HttpPost("do-survey")]
    [HttpPost("do-survey")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorDetail))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorDetail))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorDetail))]
    public async Task<ActionResult> DoSurveyAsync(DoSurveyRequest request)
    {
        await _surveyService.DoSurveyAsync(request);
        return Ok("Your answer has been recorded.");
    }

    /// <summary>
    /// User can view specific survey detail information. This API does not separate common user or admin user 
    /// </summary>
    /// <param name="id"></param>
    ///<returns></returns>
    [HttpGet("{id:long}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SurveyDetailResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorDetail))]
    public async Task<ActionResult<SurveyDetailResponse>> GetByIdAsync(long id)
    {
        return Ok(await _surveyService.GetByIdAsync(id));
    }

    /// <summary>
    /// Nguời dùng thông thường có thể filter khảo sát có status Active hoặc expired.
    /// </summary>
    /// <param name="status"></param>
    /// <param name="title"></param>
    /// <param name="sortTitle"></param>
    /// <param name="sortTotalQuestion"></param>
    /// <param name="sortPoint"></param>
    /// <param name="sortStartDate"></param>
    /// <param name="sortExpiredDate"></param>
    /// <param name="page"></param>
    /// <param name="size"></param>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PagingResponse<CommonSurveyResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorDetail))]
    public async Task<ActionResult<PagingResponse<CommonSurveyResponse>>> CommonFilterAsync(
        [FromQuery] string? status,
        [FromQuery] string? title,
        [FromQuery] string? sortTitle,
        [FromQuery] string? sortTotalQuestion,
        [FromQuery] string? sortPoint,
        [FromQuery] string? sortStartDate,
        [FromQuery] string? sortExpiredDate,
        [FromQuery] int? page,
        [FromQuery] int? size
    )
    {
        return Ok(await _surveyService.FilterCommonSurveyAsync(status, title, sortTitle, sortTotalQuestion, sortPoint,
            sortStartDate, sortExpiredDate, page, size));
    }

    /// <summary>
    /// Admin có thể filer toàn bộ danh sách kháo sát.
    /// </summary>
    /// <param name="status"></param>
    /// <param name="isDelete"></param>
    /// <param name="packType"></param>
    /// <param name="title"></param>
    /// <param name="sortTitle"></param>
    /// <param name="sortCreatedDate"></param>
    /// <param name="sortStartDate"></param>
    /// <param name="sortExpiredDate"></param>
    /// <param name="sortModifiedDate"></param>
    /// <param name="page"></param>
    /// <param name="size"></param>
    /// <returns></returns>
    [HttpGet("/api/v1/admin/surveys")]
    [Authorize(Policy = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PagingResponse<CommonSurveyResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorDetail))]
    public async Task<ActionResult<PagingResponse<SurveyResponse>>> FilterAsync(
        [FromQuery] string? status,
        [FromQuery] bool? isDelete,
        [FromQuery] string? packType,
        [FromQuery] string? title,
        [FromQuery] string? sortTitle,
        [FromQuery] string? sortCreatedDate,
        [FromQuery] string? sortStartDate,
        [FromQuery] string? sortExpiredDate,
        [FromQuery] string? sortModifiedDate,
        [FromQuery] int? page,
        [FromQuery] int? size)
    {
        return Ok(await _surveyService.FilterSurveyAsync(status, isDelete, packType, title, sortTitle, sortCreatedDate,
            sortStartDate, sortExpiredDate, sortModifiedDate, page, size));
    }

    /// <summary>
    /// Người dùng có thể filter các khảo sát đã tạo
    /// </summary>
    /// <param name="status"></param>
    /// <param name="packType"></param>
    /// <param name="title"></param>
    /// <param name="sortTitle"></param>
    /// <param name="sortCreatedDate"></param>
    /// <param name="sortStartDate"></param>
    /// <param name="sortExpiredDate"></param>
    /// <param name="sortModifiedDate"></param>
    /// <param name="page"></param>
    /// <param name="size"></param>
    /// <returns></returns>
    [HttpGet("/api/v1/account/surveys")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PagingResponse<CommonSurveyResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorDetail))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorDetail))]
    public async Task<ActionResult<PagingResponse<SurveyResponse>>> FilterAccountSurveyAsync(
        [FromQuery] string? status,
        [FromQuery] string? packType,
        [FromQuery] string? title,
        [FromQuery] string? sortTitle,
        [FromQuery] string? sortCreatedDate,
        [FromQuery] string? sortStartDate,
        [FromQuery] string? sortExpiredDate,
        [FromQuery] string? sortModifiedDate,
        [FromQuery] int? page,
        [FromQuery] int? size
    )
    {
        return Ok(await _surveyService.FilterAccountSurveyAsync(status, packType, title, sortTitle, sortCreatedDate,
            sortStartDate, sortExpiredDate, sortModifiedDate, page, size));
    }

    /// <summary>
    /// Người dùng có thể update khảo sát có status là draft hoặc PackPurchase. Update ngày hết hạn cập nhật sau
    /// </summary>
    /// <param name="id"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPut("{id:long}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SurveyDetailResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorDetail))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorDetail))]
    [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ErrorDetail))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorDetail))]
    public async Task<ActionResult<SurveyDetailResponse>> UpdateSurveyAsync(long id, [FromBody] SurveyRequest request)
    {
        var result = await _surveyService.UpdateSurveyAsync(id, request);
        return Ok(result);
    }

    /// <summary>
    /// Người dùng có thể đổi status sang Active hoặc Deactive
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpPatch("status/{id:long}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SurveyDetailResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorDetail))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorDetail))]
    [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ErrorDetail))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorDetail))]
    public async Task<ActionResult<SurveyDetailResponse>> ChangeStatusAsync(long id)
    {
        return Ok(await _surveyService.ChangeSurveyStatusAsync(id));
    }

    [HttpDelete("{id:long}")]
    public async Task<ActionResult> DeleteSurvey(long id)
    {
        await _surveyService.DeleteSurveyAsync(id);
        return Ok();
    }
}