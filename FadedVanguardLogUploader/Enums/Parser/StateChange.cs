namespace EVTCLogUploader.Enums
{
    public enum StateChange : byte
    {
        None,

        EnterCombat,
        ExitCombat,

        ChangeUp,
        ChangeDead,
        ChangeDown,

        Spawn,
        Despawn,
        HealthUpdate,

        LogStart,
        LogEnd,

        WeaponSwap,
        MaxHealthUpdate,
        PointOfView,

        Language,
        GwBuild,
        ShardId,

        Reward,
        BuffInitial,

        Pos,
        Velocity,
        Facing,

        TeamChange,
        AttackTarget,
        TargetAble,

        MapId,
        ReplInfo,
        StackActive,
        StackReset,

        Guild,
        BuffInfo,
        BuffFormula,

        SkillInfo,
        SkillTiming,

        BreakbarState,
        BreakbarPercent,

        Error,
        Tag,
        BarrierUpdate,

        StatReset,
        Extension,
        APIDelayed,
        InstanceStart,
        TickRate,
        Last90BBeforeDown,
        Effect,
        IDToGuid,
        Unkown
    }
}
