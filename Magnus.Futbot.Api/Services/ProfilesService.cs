﻿using AutoMapper;
using Magnus.Futbot.Common;
using Magnus.Futbot.Common.Models.DTOs;
using Magnus.Futbot.Common.Models.DTOs.Profiles;
using Magnus.Futbot.Common.Models.Responses;
using Magnus.Futbot.Common.Models.Selenium.Profiles;
using Magnus.Futbot.Database.Models;
using Magnus.Futbot.Database.Repositories;
using Magnus.Futbot.Selenium.Services.Players;
using Magnus.Futbot.Services;
using MongoDB.Bson;

namespace Magnus.Futbot.Api.Services
{
    public class ProfilesService
    {
        private readonly IMapper _mapper;
        private readonly ProfilesRepository _profilesRepository;
        private readonly FullPlayersDataService _fullPlayersDataService;
        private readonly InitProfileService _initProfileService;
        private readonly LoginSeleniumService _loginSeleniumService;

        public ProfilesService(
            ProfilesRepository profilesRepository,
            FullPlayersDataService fullPlayersDataService,
            InitProfileService initProfileService,
            LoginSeleniumService loginSeleniumService,
            IMapper mapper)
        {
            _mapper = mapper;
            _profilesRepository = profilesRepository;
            _fullPlayersDataService = fullPlayersDataService;
            _initProfileService = initProfileService;
            _loginSeleniumService = loginSeleniumService;
        }

        public async Task<IEnumerable<ProfileDTO>> GetAll(string userId)
            => _mapper.Map<IEnumerable<ProfileDTO>>(await _profilesRepository.GetAll(new ObjectId(userId)));

        public async Task<ProfileDTO> GetByEmail(string email)
            => _mapper.Map<ProfileDTO>((await _profilesRepository.GetByEmail(email))?.FirstOrDefault());

        public async Task<ProfileDTO> GetById(string profileId)
            => _mapper.Map<ProfileDTO>((await _profilesRepository.GetById(new ObjectId(profileId)))?.FirstOrDefault());

        public async Task<LoginResponseDTO> Add(AddProfileDTO profileDTO)
        {
            if ((await _profilesRepository.GetByEmail(profileDTO.Email)).Any())
                return new LoginResponseDTO(ProfileStatusType.AlreadyAdded, _mapper.Map<ProfileDTO>(profileDTO));
            await _profilesRepository.Add(_mapper.Map<ProfileDocument>(profileDTO));

            var response = await _initProfileService.InitProfile(profileDTO);
            await _profilesRepository.Update(_mapper.Map<ProfileDocument>(response));

            return new LoginResponseDTO(response.Status, _mapper.Map<ProfileDTO>(profileDTO));
        }

        public async Task<ProfileDTO> SubmitCode(SubmitCodeDTO submitCodeDTO)
        {
            var response = await _loginSeleniumService.SubmitCode(submitCodeDTO);
            return _mapper.Map<ProfileDTO>(await _profilesRepository.UpdateSubmitCodeStatus(submitCodeDTO.Email, response));
        }

        public async Task<ProfileDTO> RefreshProfile(string profileId, string userId)
        {
            var profile = await _profilesRepository.Get(new ObjectId(profileId), new ObjectId(userId));
            var refreshedProfile = await _initProfileService.InitProfile(_mapper.Map<ProfileDTO>(profile));
            refreshedProfile.TradePile = await _fullPlayersDataService.GetTransferPile(refreshedProfile);
            await _profilesRepository.Update(_mapper.Map<ProfileDocument>(refreshedProfile));
            return refreshedProfile;
        }

        public Task UpdateProfile(ProfileDTO profileDTO)
            => _profilesRepository.Update(_mapper.Map<ProfileDocument>(profileDTO));

        public async Task<ProfileDTO> EditProfile(EditProfileDTO editProfileDTO)
        {
            var profile = (await _profilesRepository.GetByEmail(editProfileDTO.Email)).FirstOrDefault();
            if (profile is not null)
            {
                profile.Password = editProfileDTO.Password;
                await _profilesRepository.Update(profile);
            }

            return _mapper.Map<ProfileDTO>(profile);
        }

        public async Task<ProfileDTO> EditProfileAutoRelist(EditProfileAutoListDTO editProfileAutoListDTO)
        {
            var profile = (await _profilesRepository.GetById(new ObjectId(editProfileAutoListDTO.ProfileId))).FirstOrDefault();
            if (profile is not null)
            {
                profile.AutoRelist = editProfileAutoListDTO.AutoRelist;
                await _profilesRepository.Update(profile);
            }

            return _mapper.Map<ProfileDTO>(profile);
        }

        public async Task<IEnumerable<ProfileDTO>> GetRelistProfiles()
        {
            var profiles = await _profilesRepository.GetAutoRelistProfiles();
            return _mapper.Map<IEnumerable<ProfileDTO>>(profiles);
        }
    }
}
